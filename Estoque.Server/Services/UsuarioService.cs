using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;

namespace Estoque.Server.Services;

public class UsuarioService : BaseService
{
    private readonly UsuarioRepository _usuarioRepository;
    private readonly UnidadeOrganizacionalRepository _unidadeOrganizacionalRepository;

    public UsuarioService(UsuarioRepository usuarioRepository, UnidadeOrganizacionalRepository unidadeOrganizacionalRepository)
    {
        _usuarioRepository = usuarioRepository;
        _unidadeOrganizacionalRepository = unidadeOrganizacionalRepository;
    }

    public async Task<Guid> CadastrarUsuario(Usuario usuario)
    {
        try
        {
            await ValidarUsuario(usuario, Guid.Empty);

            usuario.Perfil = PerfilUsuario.Normal;

            return await _usuarioRepository.CadastrarUsuario(usuario);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar usuário: {ex.Message}");
        }
    }

    public async Task AtualizarUsuario(Usuario usuario, Guid usuarioId)
    {
        try
        {
            await ValidarUsuario(usuario, usuarioId);

            var affected = await _usuarioRepository.AtualizarUsuario(usuario, usuarioId);

            if (affected <= 0) throw new NotFoundException("Usuário não encontrado com os parâmetros informados.");
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
        }
    }

    public async Task ValidarAcesso(Guid usuarioId)
    {
        try
        {
            var usuario = await _usuarioRepository.ObterUsuario(usuarioId);

            if (usuario == null) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            if (usuario.Valido) throw new InvalidOperationException("O acesso do usuário já está validado.");

            await _usuarioRepository.ValidarAcesso(usuarioId);
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
            throw new Exception($"Erro ao validar acesso do usuário: {ex.Message}");
        }
    }

    public async Task ExcluirUsuario(Guid usuarioId)
    {
        try
        {
            var affected = await _usuarioRepository.ExcluirUsuario(usuarioId);

            if (affected <= 0) throw new NotFoundException("Usuário não encontrado para o ID informado.");
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir usuário: {ex.Message}");
        }
    }

    public async Task<List<UsuarioGetResponse>> ObterUsuarios(int skip, int top, string? username, Guid? unidadeOrganizacionalId)
    {
        try
        {
            return await _usuarioRepository.ObterUsuarios(skip, top, username, unidadeOrganizacionalId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar usuários: {ex.Message}");
        }
    }

    public async Task<UsuarioGetResponse> ObterUsuario(Guid usuarioId)
    {
        try
        {
            var usuario = await _usuarioRepository.ObterUsuario(usuarioId);

            if (usuario == null) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            return usuario;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar usuário por ID: {ex.Message}");
        }
    }

    private async Task ValidarUsuario(Usuario usuario, Guid usuarioId)
    {
        if (usuario.UnidadeOrganizacionalId == null || usuario.UnidadeOrganizacionalId == Guid.Empty)
        {
            AddError(nameof(usuario.UnidadeOrganizacionalId), "Informe a unidade organizacional.");
        }
        else
        {
            var unidadeOrganizacionalExiste = await _unidadeOrganizacionalRepository.ObterUnidade(usuario.UnidadeOrganizacionalId.Value);

            if (unidadeOrganizacionalExiste == null)
                AddError(nameof(usuario.UnidadeOrganizacionalId), "Unidade organizacional não encontrada.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Username))
        {
            AddError(nameof(usuario.Username), "Informe o username");
        }
        else
        {
            var usernameExiste = await _usuarioRepository.VerificarUsuarioExiste(usuario.Username, usuario.UnidadeOrganizacionalId, usuarioId);

            if (usernameExiste)
                AddError(nameof(usuario.Username), "Este username já está sendo utilizado por outro usuário.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Senha))
            AddError(nameof(usuario.Senha), "Informe a senha.");

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            AddError(nameof(usuario.Nome), "Informe o nome.");

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}