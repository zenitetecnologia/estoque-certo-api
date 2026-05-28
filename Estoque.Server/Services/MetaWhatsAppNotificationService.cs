using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Estoque.Server.Services;

public class MetaWhatsAppNotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private readonly string _phoneNumberId;
    private readonly string _templateName;
    private readonly string _languageCode;
    private readonly string _graphApiVersion;
    private readonly bool _enabled;

    public MetaWhatsAppNotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _accessToken = Environment.GetEnvironmentVariable("META_WHATSAPP_ACCESS_TOKEN") ?? string.Empty;
        _phoneNumberId = Environment.GetEnvironmentVariable("META_WHATSAPP_PHONE_NUMBER_ID") ?? string.Empty;
        _templateName = Environment.GetEnvironmentVariable("META_WHATSAPP_TEMPLATE_NAME") ?? string.Empty;
        _languageCode = Environment.GetEnvironmentVariable("META_WHATSAPP_LANGUAGE_CODE") ?? "pt_BR";
        _graphApiVersion = Environment.GetEnvironmentVariable("META_GRAPH_API_VERSION") ?? "v23.0";

        _enabled = !string.IsNullOrWhiteSpace(_accessToken) &&
                   !string.IsNullOrWhiteSpace(_phoneNumberId) &&
                   !string.IsNullOrWhiteSpace(_templateName);
    }

    public Task EnviarCodigoRecuperacaoSenhaAsync(string telefoneE164, string codigo)
    {
        return EnviarTemplateAsync(telefoneE164, codigo, "5");
    }

    public Task EnviarConfirmacaoSenhaRedefinidaAsync(string telefoneE164)
    {
        Console.WriteLine($"[Meta WhatsApp] Confirmação de senha redefinida não enviada para {telefoneE164}: template não configurado.");
        return Task.CompletedTask;
    }

    public Task EnviarConfirmacaoSenhaCriadaAsync(string telefoneE164)
    {
        Console.WriteLine($"[Meta WhatsApp] Confirmação de senha criada não enviada para {telefoneE164}: template não configurado.");
        return Task.CompletedTask;
    }

    private async Task EnviarTemplateAsync(string telefoneE164, string codigo, string minutosExpiracao)
    {
        if (!_enabled)
        {
            Console.WriteLine($"[Meta WhatsApp desabilitado] Enviaria para {telefoneE164}: código {codigo}");
            return;
        }

        var payload = new
        {
            messaging_product = "whatsapp",
            to = NormalizarTelefoneParaMeta(telefoneE164),
            type = "template",
            template = new
            {
                name = _templateName,
                language = new
                {
                    code = _languageCode
                },
                components = new object[]
                {
                    new
                    {
                        type = "body",
                        parameters = new object[]
                        {
                            new { type = "text", text = codigo },
                            new { type = "text", text = minutosExpiracao }
                        }
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://graph.facebook.com/{_graphApiVersion}/{_phoneNumberId}/messages"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Erro ao enviar WhatsApp pela Meta: {responseBody}");
        }
    }

    private static string NormalizarTelefoneParaMeta(string telefoneE164)
    {
        return new string(telefoneE164.Where(char.IsDigit).ToArray());
    }
}