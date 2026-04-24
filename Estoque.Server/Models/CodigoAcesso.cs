namespace Estoque.Server.Models;

public class CodigoAcesso
{
    public Guid UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataValidacao { get; set; }
    public bool Utilizado { get; set; }
    public string? CodigoAcessoId { get; set; }
    public DateTime? DataAcessoId { get; set; }
    public bool ResetEfetuado { get; set; }
}