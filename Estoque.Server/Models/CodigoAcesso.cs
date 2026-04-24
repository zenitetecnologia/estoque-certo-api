namespace Estoque.Server.Models;

public class CodigoAcesso
{
    public Guid UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataValidacao { get; set; }
    public bool Validado { get; set; }
    public string? CodigoAcessoId { get; set; }
}