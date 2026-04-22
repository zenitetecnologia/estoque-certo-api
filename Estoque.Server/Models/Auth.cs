using System.Text.Json.Serialization;

namespace Estoque.Server.Models;

public class Auth
{
    [JsonPropertyOrder(1)]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyOrder(2)]
    public virtual string Senha { get; set; } = string.Empty;

    [JsonPropertyOrder(3)]
    public virtual Guid? UnidadeOrganizacionalId { get; set; }

    [JsonIgnore]
    [JsonPropertyOrder(4)]
    public virtual string Code { get; set; } = string.Empty;
}

public class AuthForgot : Auth
{
    [JsonIgnore]
    [JsonPropertyOrder(2)]
    public override string Senha { get; set; } = string.Empty;

    [JsonIgnore]
    [JsonPropertyOrder(3)]
    public override Guid? UnidadeOrganizacionalId { get; set; }
}

public class AuthVerify : Auth
{
    [JsonIgnore]
    [JsonPropertyOrder(2)]
    public override string Senha { get; set; } = string.Empty;

    [JsonIgnore]
    [JsonPropertyOrder(3)]
    public override Guid? UnidadeOrganizacionalId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyOrder(4)]
    public override string Code { get; set; } = string.Empty;
}

public class AuthReset : Auth
{
    [JsonIgnore]
    [JsonPropertyOrder(3)]
    public override Guid? UnidadeOrganizacionalId { get; set; }
}

public class AuthToken
{
    public string Token { get; set; } = string.Empty;
}