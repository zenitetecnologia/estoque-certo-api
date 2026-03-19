using Estoque.Controllers;

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
builder.Services.AddScoped<Estoque.Repositories.EspacosRepository>();
builder.Services.AddScoped<Estoque.Services.IEspacosService, Estoque.Services.EspacosService>();
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

app.Run();