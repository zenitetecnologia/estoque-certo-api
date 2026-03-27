using Estoque.models;
using System.Text.Json.Serialization;

namespace Estoque.Server.Models;

public class Usuario
{
    [JsonIgnore]
    public virtual int UsuarioId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public virtual int UnidadeOrganizacionalId { get; set; }

    [JsonIgnore]
    public virtual PerfilUsuario Perfil { get; set; }

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
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public override int UsuarioId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public override PerfilUsuario Perfil { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public override bool Valido { get; set; }
}