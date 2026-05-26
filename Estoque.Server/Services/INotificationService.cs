namespace Estoque.Server.Services;

public interface INotificationService
{
    Task EnviarCodigoRecuperacaoSenhaAsync(string telefoneE164, string codigo);
    Task EnviarConfirmacaoSenhaRedefinidaAsync(string telefoneE164);
    Task EnviarConfirmacaoSenhaCriadaAsync(string telefoneE164);
}