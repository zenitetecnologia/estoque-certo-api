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

    public async Task<AuthResponde> Login(Auth auth)
    {
        try
        {
            ValidarAuth(auth);

            var usuario = await _usuarioRepository.ObterIdentificador(auth.Identificador);

            if (usuario == null)
                throw new UnauthorizedAccessException("Usuário ou telefone não encontrado.");

            if (usuario.Senha != auth.Senha)
                throw new UnauthorizedAccessException("Senha incorreta.");

            if (!usuario.Valido)
                throw new Exception("FORBIDDEN:Usuário não validado.");

            return new AuthResponde
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

    public async Task EsquecerSenha(ForgotRequest request)
    {
        try
        {
            ValidarForgot(request);

            var usuario = await _usuarioRepository.ObterIdentificador(request.Identificador);

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

    private void ValidarForgot(ForgotRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Identificador))
        {
            AddError(nameof(request.Identificador), "Informe o username ou telefone.");
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
                new Claim("UnidadeOrganizacionalId", usuario.UnidadeOrganizacionalId.ToString()!)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private void ValidarAuth(Auth auth)
    {
        if (string.IsNullOrWhiteSpace(auth.Identificador))
        {
            AddError(nameof(auth.Identificador), "Informe o username ou telefone.");
        }

        if (string.IsNullOrWhiteSpace(auth.Senha))
        {
            AddError(nameof(auth.Senha), "Informe a senha.");
        }

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}