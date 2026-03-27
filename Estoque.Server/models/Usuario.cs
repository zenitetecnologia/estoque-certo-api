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
    public virtual PerfilUsuario Perfil { get; set; }
    public virtual int UnidadeOrganizacionalId { get; set; }
    [JsonIgnore]
    public virtual bool Valido { get; set; }
}

public class UsuarioAtualizado : Usuario
{
    [JsonIgnore]
    public override int UnidadeOrganizacionalId { get; set; }
}

public class UsuarioRecuperado : Usuario
{
    public int UsuarioId { get; set; }
    public override PerfilUsuario Perfil { get; set; }
    public override bool Valido { get; set; }
}