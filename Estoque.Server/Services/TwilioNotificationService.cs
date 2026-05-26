using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Estoque.Server.Services;

public class TwilioNotificationService : INotificationService
{
    private readonly string _fromNumber;
    private readonly bool _enabled;

    public TwilioNotificationService()
    {
        var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER") ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(accountSid) &&
            !string.IsNullOrWhiteSpace(authToken) &&
            !string.IsNullOrWhiteSpace(_fromNumber))
        {
            TwilioClient.Init(accountSid, authToken);
            _enabled = true;
        }
        else
        {
            _enabled = false;
        }
    }

    public Task EnviarCodigoRecuperacaoSenhaAsync(string telefoneE164, string codigo)
    {
        var body = $"Seu código de recuperação do Estoque Certo é: {codigo}. " +
                   "Ele expira em 5 minutos.";
        return EnviarMensagemAsync(telefoneE164, body);
    }

    public Task EnviarConfirmacaoSenhaRedefinidaAsync(string telefoneE164)
    {
        var body = "Sua senha do Estoque Certo foi redefinida com sucesso. " +
                   "Se não foi você, contate o suporte.";
        return EnviarMensagemAsync(telefoneE164, body);
    }

    public Task EnviarConfirmacaoSenhaCriadaAsync(string telefoneE164)
    {
        var body = "Sua conta do Estoque Certo foi criada com sucesso.";
        return EnviarMensagemAsync(telefoneE164, body);
    }

    private async Task EnviarMensagemAsync(string telefoneE164, string body)
    {
        // Enquanto estiver sem config/produção, apenas loga
        if (!_enabled)
        {
            Console.WriteLine($"[Twilio desabilitado] Enviaria para {telefoneE164}: {body}");
            return;
        }

        // Para WhatsApp: TWILIO_FROM_NUMBER = "whatsapp:+55XXXXXXXXXXX"
        // e telefoneE164 = "whatsapp:+55DDDNUMERO"
        await MessageResource.CreateAsync(
            to: new PhoneNumber(telefoneE164),
            from: new PhoneNumber(_fromNumber),
            body: body
        );
    }
}