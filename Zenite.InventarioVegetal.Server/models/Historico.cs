namespace Zenite.InventarioVegetal.Server.Models
{
    public class Historico
    {
        public int IdItemEstoque { get; set; }
        public int IdUsuario { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
        public decimal QuantidadeAnterior { get; set; }
        public decimal QuantidadeResultante { get; set; }
    }
}