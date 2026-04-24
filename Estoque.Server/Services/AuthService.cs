using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Estoque.Server.Services;

public class AuthService : BaseService
{
    private readonly UsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(UsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<AuthToken> Login(Auth auth)
    {
        try
        {
            ValidarAuth(auth);

            var usuario = await _usuarioRepository.ObterIdentificador(auth.Username);

            if (usuario == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            if (usuario.Senha != auth.Senha)
                throw new UnauthorizedAccessException("Usuário ou senha incorreta.");

            if (usuario.UnidadeOrganizacionalId != auth.UnidadeOrganizacionalId)
                throw new UnauthorizedAccessException("Usuário não encontrado para unidade organizacional informada.");

            if (!usuario.Valido)
                throw new Exception("FORBIDDEN:Usuário não validado.");

            return new AuthToken
            {
                Token = GerarTokenJwt(usuario)
            };
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex) when (ex.Message.StartsWith("FORBIDDEN:"))
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao realizar login: {ex.Message}");
        }
    }

    public async Task EsquecerSenha(Auth auth)
    {
        try
        {
            ValidarAuth(auth);

            var usuario = await _usuarioRepository.ObterIdentificador(auth.Username);

            if (usuario == null)
                throw new NotFoundException("Usuário não encontrado.");

            string codigoGerado = new Random().Next(100000, 999999).ToString();

            await _usuarioRepository.InserirCodigoAcesso(usuario.UsuarioId, codigoGerado);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao solicitar recuperação: {ex.Message}");
        }
    }

    public async Task<string> VerificarCodigo(AuthVerify auth)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(auth.Code))
            {
                AddError("Code", "Informe o código.");
                throw new ValidationException(Errors);
            }

            var codigoAcesso = await _usuarioRepository.ObterCodigoPorSms(auth.Code);

            if (codigoAcesso == null)
                throw new NotFoundException("Código inválido ou não encontrado.");

            if (codigoAcesso.Validado)
                throw new InvalidOperationException("Este código já foi validado anteriormente.");

            if (DateTime.UtcNow > codigoAcesso.DataSolicitacao.AddMinutes(5))
                throw new InvalidOperationException("O tempo de validação deste código expirou.");

            bool existeMaisRecente = await _usuarioRepository.BuscarUltimoCodigo(codigoAcesso.UsuarioId, codigoAcesso.DataSolicitacao);
            if (existeMaisRecente)
                throw new InvalidOperationException("Este código foi invalidado porque uma nova solicitação foi feita.");

            string codigoGigante = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)).ToLower();

            await _usuarioRepository.AtualizarCodigoValidado(codigoAcesso.UsuarioId, codigoAcesso.Codigo, codigoGigante);

            return codigoGigante;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao verificar código: {ex.Message}");
        }
    }

    public async Task RedefinirSenha(AuthReset auth)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(auth.CodigoAcessoId))
                AddError("CodigoAcessoId", "Código de acesso não informado.");

            if (string.IsNullOrWhiteSpace(auth.Senha))
                AddError("Senha", "Informe a nova senha.");

            if (Errors.Any())
                throw new ValidationException(Errors);

            var codigoAcesso = await _usuarioRepository.ObterCodigoPorId(auth.CodigoAcessoId);

            if (codigoAcesso == null)
                throw new NotFoundException("Código de acesso inválido ou não encontrado.");

            if (!codigoAcesso.Validado)
                throw new InvalidOperationException("Este código não foi validado. Volte e informe os 6 dígitos primeiro.");

            if (DateTime.UtcNow > codigoAcesso.DataSolicitacao.AddMinutes(15))
                throw new InvalidOperationException("O tempo limite para redefinir a senha expirou.");

            bool existeMaisRecente = await _usuarioRepository.BuscarUltimoCodigo(codigoAcesso.UsuarioId, codigoAcesso.DataSolicitacao);
            if (existeMaisRecente)
                throw new InvalidOperationException("Esta sessão expirou porque um novo código foi solicitado recentemente.");

            await _usuarioRepository.RedefinirSenha(codigoAcesso.UsuarioId, auth.Senha);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao redefinir senha: {ex.Message}");
        }
    }

    private void ValidarAuth(Auth auth)
    {
        if (string.IsNullOrWhiteSpace(auth.Username))
            AddError(nameof(auth.Username), "Informe o username ou telefone.");

        if (auth.GetType() == typeof(Auth))
        {
            if (string.IsNullOrWhiteSpace(auth.Senha))
                AddError(nameof(auth.Senha), "Informe a senha.");
        }

        if (auth.UnidadeOrganizacionalId == null || auth.UnidadeOrganizacionalId == Guid.Empty)
            AddError(nameof(auth.UnidadeOrganizacionalId), "Informe a unidade organizacional.");

        if (Errors.Any())
            throw new ValidationException(Errors);
    }

    private string GerarTokenJwt(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var keyString = _configuration["Jwt:Key"] ?? "yuri_lucas_zenite_tecnologia_2026_auth";
        var key = Encoding.ASCII.GetBytes(keyString);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim("UnidadeOrganizacionalId", usuario.UnidadeOrganizacionalId.ToString()!),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
                new Claim("Valido", usuario.Valido.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}