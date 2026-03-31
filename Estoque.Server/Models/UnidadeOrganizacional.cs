using System.Text.Json.Serialization;

namespace Estoque.models;

public class UnidadeOrganizacional
{
    [JsonIgnore]
    [JsonPropertyOrder(-1)]
    public virtual Guid UnidadeOrganizacionalId { get; set; }


    public Guid? IdMatriz { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UnidadeOrganizacionalRecuperado : UnidadeOrganizacional
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyOrder(-1)]
    public override Guid UnidadeOrganizacionalId { get; set; }
}