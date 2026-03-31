namespace Estoque.models
{
    public class ItemEstoque
    {
        public int ItemEstoqueId { get; set; }
        public int UnidadeOrganizacionalId { get; set; }
        public int Espaco { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public TipoUnidadeMedida TipoUnidadeMedida { get; set; }
        public decimal Quantidade { get; set; }
    }
}