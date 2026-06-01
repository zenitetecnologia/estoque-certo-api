using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Estoque.Server.Services;

public class PayloadCryptoService : IDisposable
{
    private readonly RSA _rsa = RSA.Create(2048);
    private readonly string _publicKeyBase64;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PayloadCryptoService()
    {
        var privateKey = Environment.GetEnvironmentVariable("encrypt_private_key");
        var publicKey = Environment.GetEnvironmentVariable("encrypt_public_key");

        if (string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(publicKey))
            throw new InvalidOperationException("As variáveis de ambiente encrypt_private_key e encrypt_public_key não foram encontradas ou configuradas.");

        var privateKeyBytes = Convert.FromBase64String(privateKey);
        _rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        _publicKeyBase64 = publicKey;
    }

    public string ObterChavePublica()
    {
        return _publicKeyBase64;
    }

    public T Descriptografar<T>(string payload)
    {
        return Descriptografar<T>(new Models.EncryptedRequest
        {
            Payload = payload
        });
    }

    public T Descriptografar<T>(Models.EncryptedRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Payload))
            throw new InvalidOperationException("Payload criptografado não informado.");

        try
        {
            var decryptedBytes = string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Iv)
                ? DecryptRsaPayload(request.Payload)
                : DecryptHybridPayload(request);

            var json = Encoding.UTF8.GetString(decryptedBytes);

            return JsonSerializer.Deserialize<T>(json, _jsonOptions)
                ?? throw new InvalidOperationException("Payload criptografado inválido.");
        }
        catch (CryptographicException)
        {
            throw new InvalidOperationException("Payload criptografado inválido.");
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("Payload criptografado inválido.");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Payload criptografado inválido.");
        }
    }

    private byte[] DecryptRsaPayload(string payload)
    {
        var encryptedBytes = Convert.FromBase64String(payload);
        return _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
    }

    private byte[] DecryptHybridPayload(Models.EncryptedRequest request)
    {
        var encryptedKey = Convert.FromBase64String(request.Key);
        var key = _rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
        var iv = Convert.FromBase64String(request.Iv);
        var payloadBytes = Convert.FromBase64String(request.Payload);

        const int tagSize = 16;
        if (payloadBytes.Length <= tagSize)
            throw new InvalidOperationException("Payload criptografado inválido.");

        var cipherTextLength = payloadBytes.Length - tagSize;
        var cipherText = payloadBytes[..cipherTextLength];
        var tag = payloadBytes[cipherTextLength..];
        var plainText = new byte[cipherTextLength];

        using var aes = new AesGcm(key, tagSize);
        aes.Decrypt(iv, cipherText, tag, plainText);

        return plainText;
    }

    public void Dispose()
    {
        _rsa.Dispose();
    }
}