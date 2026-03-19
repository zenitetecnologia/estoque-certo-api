using Estoque.Controllers;
using Estoque.Server.Validations;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<System.Data.IDbConnection>(sp =>
    new Npgsql.NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

//usuario
builder.Services.AddScoped<Estoque.Repositories.UsuarioRepository>();
builder.Services.AddScoped<Estoque.Services.IUsuarioService, Estoque.Services.UsuarioService>();
//item estoque
builder.Services.AddScoped<Estoque.Repositories.ItemEstoqueRepository>();
builder.Services.AddScoped<Estoque.Services.IItemEstoqueService, Estoque.Services.ItemEstoqueService>();
//unidade organizacional
builder.Services.AddScoped<Estoque.Repositories.UnidadeOrganizacionalRepository>();
builder.Services.AddScoped<Estoque.Services.IUnidadeOrganizacionalService, Estoque.Services.UnidadeOrganizacionalService>();
//espacos
builder.Services.AddScoped<Estoque.Repositories.EspacoRepository>();
builder.Services.AddScoped<Estoque.Services.IEspacosService, Estoque.Services.EspacoService>();
//historico
builder.Services.AddScoped<Estoque.Repositories.HistoricoRepository>();
builder.Services.AddScoped<Estoque.Services.IHistoricoService, Estoque.Services.HistoricoService>();

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

// app.UseHttpsRedirection();

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

        if (exception is ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(validationEx.Errors);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message
        });
    });
});

app.Run();