using System.Text.Json.Serialization;

namespace Estoque.models;

public class Espaco
{
    [JsonIgnore]
    [JsonPropertyOrder(1)]
    public virtual Guid EspacoId { get; set; }

    [JsonPropertyOrder(2)]
    public virtual Guid UnidadeOrganizacionalId { get; set; }

    [JsonPropertyOrder(3)]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyOrder(4)]
    public string Descricao { get; set; } = string.Empty;
}

public class EspacoAtualizado : Espaco
{
    [JsonIgnore]
    [JsonPropertyOrder(2)]
    public override Guid UnidadeOrganizacionalId { get; set; }
}

public class EspacoRecuperado : Espaco
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyOrder(1)]
    public override Guid EspacoId { get; set; }
}