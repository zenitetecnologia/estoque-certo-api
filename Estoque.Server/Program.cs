var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<System.Data.IDbConnection>(sp =>
    new Npgsql.NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<Estoque.Repositories.UsuarioRepository>();
builder.Services.AddScoped<Estoque.Services.IUsuarioService, Estoque.Services.UsuarioService>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();