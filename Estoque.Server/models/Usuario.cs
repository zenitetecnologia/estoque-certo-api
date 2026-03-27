using Estoque.models;
using System.Text.Json.Serialization;

namespace Estoque.Server.Models;

public class Usuario
{
    [JsonIgnore]
    [JsonPropertyOrder(1)]
    public virtual int UsuarioId { get; set; }

    [JsonPropertyOrder(2)]
    public virtual int UnidadeOrganizacionalId { get; set; }

    [JsonPropertyOrder(3)]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyOrder(4)]
    public string Senha { get; set; } = string.Empty;

    [JsonPropertyOrder(5)]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public string Telefone { get; set; } = string.Empty;

    [JsonIgnore]
    [JsonPropertyOrder(7)]
    public virtual PerfilUsuario Perfil { get; set; }

    [JsonIgnore]
    [JsonPropertyOrder(8)]
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
    [JsonPropertyOrder(1)]
    public override int UsuarioId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyOrder(7)]
    public override PerfilUsuario Perfil { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyOrder(8)]
    public override bool Valido { get; set; }
}