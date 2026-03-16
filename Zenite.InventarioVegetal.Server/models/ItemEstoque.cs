namespace Estoque.models
{
    public class ItemEstoque
    {
        public int Id { get; set; }
        public int IdUnidadeOrganizacional { get; set; }
        public int Espaco { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public TipoUnidadeMedida TipoUnidadeMedida { get; set; }
        public decimal Quantidade { get; set; }
    }
}
