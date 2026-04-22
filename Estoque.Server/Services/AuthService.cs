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

            var usuario = await _usuarioRepository.ObterIdentificador(auth.Login);

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

            var usuario = await _usuarioRepository.ObterIdentificador(auth.Login);

            if (usuario == null)
                throw new NotFoundException("Usuário não encontrado.");
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

    public async Task VerificarCodigo(Auth auth)
    {
        try
        {
            ValidarAuth(auth);

            bool codigoNaoEncontrado = false;
            bool tempoExpirado = false;

            if (codigoNaoEncontrado)
                throw new NotFoundException("Código não encontrado ou inválido.");

            if (tempoExpirado)
                throw new InvalidOperationException("O tempo de validação deste código expirou.");
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

    private void ValidarAuth(Auth auth)
    {
        if (string.IsNullOrWhiteSpace(auth.Login))
        {
            AddError(nameof(auth.Login), "Informe o username ou telefone.");
        }

        if (auth is not AuthForgot && auth is not AuthVerify)
        {
            if (string.IsNullOrWhiteSpace(auth.Senha))
            {
                AddError(nameof(auth.Senha), "Informe a senha.");
            }
        }

        if (auth.GetType() == typeof(Auth))
        {
            if (auth.UnidadeOrganizacionalId == null || auth.UnidadeOrganizacionalId == Guid.Empty)
                AddError(nameof(auth.UnidadeOrganizacionalId), "Informe a unidade organizacional.");
        }

        if (auth is AuthVerify)
        {
            if (string.IsNullOrWhiteSpace(auth.Code))
            {
                AddError(nameof(auth.Code), "Informe o código.");
            }
        }

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

    public async Task RedefinirSenha(Auth auth)
    {
        try
        {
            ValidarAuth(auth);

            var usuario = await _usuarioRepository.ObterIdentificador(auth.Login);

            if (usuario == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            await _usuarioRepository.RedefinirSenha(usuario.UsuarioId, auth.Senha);
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
            throw new Exception($"Erro ao redefinir senha: {ex.Message}");
        }
    }
}