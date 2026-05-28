using System.Net.Http.Headers;
using System.Text;

namespace Estoque.Server.Services;

public class TwilioNotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;
    private readonly string _channel;
    private readonly string _contentSid;
    private readonly bool _removeNonoDigitoBrasil;
    private readonly bool _enabled;

    public TwilioNotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID") ?? string.Empty;
        _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN") ?? string.Empty;
        _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER") ?? string.Empty;
        _channel = Environment.GetEnvironmentVariable("TWILIO_CHANNEL") ?? "sms";
        _contentSid = Environment.GetEnvironmentVariable("TWILIO_CONTENT_SID") ?? string.Empty;
        _removeNonoDigitoBrasil = string.Equals(
            Environment.GetEnvironmentVariable("TWILIO_REMOVE_NONO_DIGITO_BRASIL"),
            "true",
            StringComparison.OrdinalIgnoreCase
        );

        _enabled = !string.IsNullOrWhiteSpace(_accountSid) &&
                   !string.IsNullOrWhiteSpace(_authToken) &&
                   !string.IsNullOrWhiteSpace(_fromNumber);
    }

    public Task EnviarCodigoRecuperacaoSenhaAsync(string telefoneE164, string codigo)
    {
        var body = $"Seu código de recuperação do Estoque Certo é: {codigo}. Ele expira em 5 minutos.";
        return EnviarMensagemAsync(telefoneE164, body, new Dictionary<string, string>
        {
            ["1"] = codigo,
            ["2"] = "5"
        });
    }

    public Task EnviarConfirmacaoSenhaRedefinidaAsync(string telefoneE164)
    {
        var body = "Sua senha do Estoque Certo foi redefinida com sucesso. Se não foi você, contate o suporte.";
        return EnviarMensagemAsync(telefoneE164, body);
    }

    public Task EnviarConfirmacaoSenhaCriadaAsync(string telefoneE164)
    {
        var body = "Sua conta do Estoque Certo foi criada com sucesso.";
        return EnviarMensagemAsync(telefoneE164, body);
    }

    private async Task EnviarMensagemAsync(string telefoneE164, string body, Dictionary<string, string>? contentVariables = null)
    {
        if (!_enabled)
        {
            Console.WriteLine($"[Twilio desabilitado] Enviaria para {telefoneE164}: {body}");
            return;
        }

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/Messages.json"
        );

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        var form = new Dictionary<string, string>
        {
            ["To"] = FormatarDestino(telefoneE164),
            ["From"] = _fromNumber,
        };

        if (UsaWhatsAppTemplate())
        {
            form["ContentSid"] = _contentSid;

            if (contentVariables != null && contentVariables.Count > 0)
            {
                form["ContentVariables"] = System.Text.Json.JsonSerializer.Serialize(contentVariables);
            }
        }
        else
        {
            form["Body"] = body;
        }

        request.Content = new FormUrlEncodedContent(form);

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Erro ao enviar SMS pelo Twilio: {responseBody}");
        }
    }

    private bool UsaWhatsAppTemplate()
    {
        return _channel.Equals("whatsapp", StringComparison.OrdinalIgnoreCase) &&
               !string.IsNullOrWhiteSpace(_contentSid);
    }

    private string FormatarDestino(string telefoneE164)
    {
        if (!_channel.Equals("whatsapp", StringComparison.OrdinalIgnoreCase))
            return telefoneE164;

        var telefone = _removeNonoDigitoBrasil
            ? RemoverNonoDigitoBrasil(telefoneE164)
            : telefoneE164;

        return telefone.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase)
            ? telefone
            : $"whatsapp:{telefone}";
    }

    private static string RemoverNonoDigitoBrasil(string telefoneE164)
    {
        var prefixoWhatsapp = "whatsapp:";
        var telefone = telefoneE164.StartsWith(prefixoWhatsapp, StringComparison.OrdinalIgnoreCase)
            ? telefoneE164[prefixoWhatsapp.Length..]
            : telefoneE164;

        var digitos = new string(telefone.Where(char.IsDigit).ToArray());

        if (digitos.Length == 13 && digitos.StartsWith("55") && digitos[4] == '9')
        {
            digitos = digitos.Remove(4, 1);
        }

        return telefone.StartsWith("+")
            ? "+" + digitos
            : digitos;
    }
}