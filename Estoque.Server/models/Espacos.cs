namespace Estoque.models;

public class Espacos
{
    public int UnidadeOrganizacionalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public class EspacosRecuperado : Espacos
{
    public int EspacoId { get; set; }
}