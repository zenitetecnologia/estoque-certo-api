using Estoque.models;
using System.Text.Json.Serialization;

namespace Estoque.Server.Models;

public class Usuario
{
    public string Username { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    [JsonIgnore]
    public PerfilUsuario Perfil { get; set; }
    public List<UnidadeOrganizacionalRecuperado> UnidadesOrganizacionais { get; set; } = new List<UnidadeOrganizacionalRecuperado>();
}

public class UsuarioValido
{
    public int UsuarioId { get; set; }
    public List<UnidadeOrganizacionalRecuperado> UnidadesOrganizacionais { get; set; } = new List<UnidadeOrganizacionalRecuperado>();
}

public class UsuarioRecuperado : Usuario
{
    public int UsuarioId { get; set; }
}