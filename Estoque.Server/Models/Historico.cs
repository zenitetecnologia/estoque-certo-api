namespace Estoque.Models
{
    public class Historico
    {
        public int HistoricoId { get; set; }
        public int ItemEstoqueId { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
        public decimal QuantidadeAnterior { get; set; }
        public decimal QuantidadeResultante { get; set; }
    }
}