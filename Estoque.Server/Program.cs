using Estoque.Server.Controllers;
using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;
using Estoque.Server.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("A variável de ambiente DefaultConnection não foi encontrada ou configurada.");

builder.Services.AddScoped<IDbConnection>(x => new Npgsql.NpgsqlConnection(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UnidadeOrganizacionalRepository>();
builder.Services.AddScoped<EspacoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ItemEstoqueRepository>();
builder.Services.AddScoped<HistoricoRepository>();
builder.Services.AddScoped<AuthRepository>();

builder.Services.AddScoped<UnidadeOrganizacionalService>();
builder.Services.AddScoped<EspacoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ItemEstoqueService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services.AddCors(x => x.AddPolicy("*", y => y.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("*");
app.UseAuthorization();

app.MapUnidadeOrganizacionalEndpoints();
app.MapEspacoEndpoints();
app.MapUsuarioEndpoints();
app.MapItemEstoqueEndpoints();
app.MapAuthEndpoints();

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
