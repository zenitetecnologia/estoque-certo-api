namespace Estoque.models
{
    public class Historico
    {
        public int Id { get; set; }
        public int IdItemEstoque { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
        public decimal QuantidadeAnterior { get; set; }
        public decimal QuantidadeResultante { get; set; }
    }
}