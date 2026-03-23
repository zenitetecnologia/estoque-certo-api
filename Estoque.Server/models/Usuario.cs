using Estoque.models;

namespace Estoque.Server.Models;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public List<UnidadeOrganizacionalVinculo> UnidadesOrganizacionais { get; set; } = new List<UnidadeOrganizacionalVinculo>();
}

public class UsuarioValido
{
    public int UsuarioId { get; set; }
    public List<UnidadeOrganizacionalVinculo> UnidadesOrganizacionais { get; set; } = new List<UnidadeOrganizacionalVinculo>();
}

public class UsuarioAtualizar
{
    public string Username { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public List<UnidadeOrganizacionalVinculo> UnidadesOrganizacionais { get; set; } = new List<UnidadeOrganizacionalVinculo>();
}