using Estoque.Models;
using Estoque.Repositories;
using Estoque.Server.Models;
using Estoque.Server.Services;
using Estoque.Server.Validations;

namespace Estoque.Services;

public class UsuarioService : BaseService
{
    private readonly UsuarioRepository _repository;

    public UsuarioService(UsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CadastrarUsuario(Usuario usuario)
    {
        try
        {
            await ValidarUsuario(usuario, Guid.Empty);

            usuario.Perfil = PerfilUsuario.Normal;

            Guid usuarioId = await _repository.CadastrarUsuario(usuario);

            return usuarioId;
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

    public async Task<int> AtualizarUsuario(Usuario usuario, Guid usuarioId)
    {
        try
        {
            await ValidarUsuario(usuario, usuarioId);

            var affected = await _repository.AtualizarUsuario(usuario, usuarioId);

            if (affected <= 0) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            return affected;
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
            var usuario = await _repository.ObterUsuario(usuarioId);

            if (usuario == null) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            if (usuario.Valido) throw new InvalidOperationException("O acesso do usuário já está validado.");

            await _repository.ValidarAcesso(usuarioId);
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

    public async Task<int> ExcluirUsuario(Guid usuarioId)
    {
        try
        {
            var affected = await _repository.ExcluirUsuario(usuarioId);

            if (affected <= 0) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            return affected;
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

    public async Task<List<UsuarioRecuperado>> ObterUsuarios()
    {
        try
        {
            return await _repository.ObterUsuarios();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar usuários: {ex.Message}");
        }
    }

    public async Task<UsuarioRecuperado> ObterUsuario(Guid usuarioId)
    {
        try
        {
            var usuario = await _repository.ObterUsuario(usuarioId);

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
        if (string.IsNullOrWhiteSpace(usuario.Username))
        {
            AddError(nameof(usuario.Username), "Informe o username");
        }
        else
        {
            bool usernameExiste = await _repository.VerificarUsuarioExiste(usuario.Username, usuario.UnidadeOrganizacionalId, usuarioId);

            if (usernameExiste)
                AddError(nameof(usuario.Username), "Este username já está sendo usado por outro usuário.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Senha))
            AddError(nameof(usuario.Senha), "Informe a senha.");

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            AddError(nameof(usuario.Nome), "Informe o nome.");

        if (string.IsNullOrWhiteSpace(usuario.Telefone))
            AddError(nameof(usuario.Telefone), "Informe o telefone.");

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}