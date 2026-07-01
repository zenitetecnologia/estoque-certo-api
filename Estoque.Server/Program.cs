using Estoque.Server.Controllers;
using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;
using Estoque.Server.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("A variável de ambiente DefaultConnection não foi encontrada ou configurada.");

builder.Services.AddScoped<IDbConnection>(x => new Npgsql.NpgsqlConnection(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

        await context.HttpContext.Response.WriteAsync("Muitas tentativas. Tente novamente mais tarde.", cancellationToken);
    };

    options.AddPolicy("login-policy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ObterChaveRateLimitPorRota(httpContext),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));
});

builder.Services.AddScoped<UnidadeOrganizacionalRepository>();
builder.Services.AddScoped<EspacoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ItemEstoqueRepository>();
builder.Services.AddScoped<HistoricoRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<RelatorioRepository>();

builder.Services.AddScoped<UnidadeOrganizacionalService>();
builder.Services.AddScoped<EspacoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ItemEstoqueService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddSingleton<PayloadCryptoService>();

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services.AddCors(x => x.AddPolicy("*", y => y.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Zenite-Message")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("*");
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    if (path.StartsWithSegments("/v1/auth") ||
        path.StartsWithSegments("/swagger") ||
        path.StartsWithSegments("/health") ||
        (path.StartsWithSegments("/v1/unidades-organizacionais") && (context.Request.Method == HttpMethods.Get || context.Request.Method == HttpMethods.Post)) ||
        (path.StartsWithSegments("/v1/usuarios") && context.Request.Method == HttpMethods.Post))
    {
        await next();
        return;
    }

    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Não autorizado.");
        return;
    }

    var token = authHeader.Substring("Bearer ".Length).Trim();

    try
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyString = Environment.GetEnvironmentVariable("zenite_jwt_auth") ?? throw new InvalidOperationException("A variável de ambiente zenite_jwt_auth não foi encontrada ou configurada.");
        var key = Encoding.ASCII.GetBytes(keyString);

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;

        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "Bearer", "unique_name", "role");
        context.User = new ClaimsPrincipal(claimsIdentity);

        await next();

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
        {
            await context.Response.WriteAsync("Acesso negado.");
        }
    }
    catch (SecurityTokenExpiredException)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Não Autorizado. Token expirado.");
        return;
    }
    catch (Exception)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Não autorizado.");
        return;
    }
});

app.UseAuthorization();

app.MapUnidadeOrganizacionalEndpoints();
app.MapEspacoEndpoints();
app.MapUsuarioEndpoints();
app.MapItemEstoqueEndpoints();
app.MapAuthEndpoints();
app.MapRelatorioEndPoint();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(validationException.Errors);
            return;
        }

        if (exception is NotFoundException notFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync(notFoundException.Message);
            return;
        }

        if (exception is InvalidOperationException invalidOperationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(invalidOperationException.Message);
            return;
        }

        if (exception is ForbiddenException forbiddenException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(forbiddenException.Message);
            return;
        }

        if (exception is UnauthorizedAccessException unauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(unauthorizedAccessException.Message);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(exception?.Message ?? "Erro não identificado.");
    });
});

app.MapHealthChecks("/health");

app.Run();
static string ObterIpCliente(HttpContext httpContext)
{
    var cloudflareIp = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
    if (!string.IsNullOrWhiteSpace(cloudflareIp))
        return cloudflareIp.Trim();

    var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
    if (!string.IsNullOrWhiteSpace(forwardedFor))
        return forwardedFor.Split(',')[0].Trim();

    var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
    if (!string.IsNullOrWhiteSpace(realIp))
        return realIp.Trim();

    return httpContext.Connection.RemoteIpAddress?.ToString() ?? "ip-nao-identificado";
}

static string ObterChaveRateLimitPorRota(HttpContext httpContext)
{
    var rota = httpContext.GetEndpoint() is RouteEndpoint routeEndpoint
        ? routeEndpoint.RoutePattern.RawText
        : httpContext.Request.Path.Value;

    return $"{ObterIpCliente(httpContext)}:{httpContext.Request.Method}:{rota ?? "rota-nao-identificada"}";
}