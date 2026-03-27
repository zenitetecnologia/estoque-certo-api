using Estoque.Controllers;
using Estoque.Repositories;
using Estoque.Server.Validations;
using Estoque.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddScoped<System.Data.IDbConnection>(sp =>
    new Npgsql.NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

//usuario
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
//item estoque
builder.Services.AddScoped<ItemEstoqueRepository>();
builder.Services.AddScoped<ItemEstoqueService>();
//unidade organizacional
builder.Services.AddScoped<UnidadeOrganizacionalRepository>();
builder.Services.AddScoped<UnidadeOrganizacionalService>();
//espacos
builder.Services.AddScoped<EspacoRepository>();
builder.Services.AddScoped<EspacoService>();
//historico
builder.Services.AddScoped<HistoricoRepository>();
builder.Services.AddScoped<HistoricoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirTudo");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapFallbackToFile("/index.html");

app.MapEspacosEndpoints();
app.MapHistoricoEndpoints();
app.MapItemEstoqueEndpoints();
app.MapUnidadeOrganizacionalEndpoints();
app.MapUsuarioEndpoints();


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

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(exception?.Message ?? "Erro não identificado.");
    });
});

app.Run();