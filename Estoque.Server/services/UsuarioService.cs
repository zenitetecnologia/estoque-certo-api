using Estoque.models;
using Estoque.Repositories;
using Estoque.Server.Models;
using Estoque.Server.Validations;

namespace Estoque.Services;

public class UsuarioService
{
    private readonly UsuarioRepository _repository;

    public UsuarioService(UsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> CadastrarUsuario(Usuario usuario)
    {
        try
        {
            await ValidarUsuario(usuario, 0);

            usuario.Perfil = PerfilUsuario.Normal;

            int usuarioId = await _repository.CadastrarUsuario(usuario);

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

    public async Task<int> AtualizarUsuario(Usuario usuario, int usuarioId)
    {
        try
        {
            var usuarioAux = await _repository.ObterUsuario(usuarioId);

            if (usuarioAux == null) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            usuario.UnidadeOrganizacionalId = usuarioAux.UnidadeOrganizacionalId;

            await ValidarUsuario(usuario, usuarioId);

            return await _repository.AtualizarUsuario(usuario, usuarioId);
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

    public async Task ValidarAcesso(int usuarioId)
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

    public async Task<bool> ExcluirUsuario(int usuarioId)
    {
        try
        {
            var usuario = await _repository.ObterUsuario(usuarioId);

            if (usuario == null) throw new NotFoundException("Usuário não encontrado para o ID informado.");

            return await _repository.ExcluirUsuario(usuarioId);
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

    public async Task<UsuarioRecuperado> ObterUsuario(int usuarioId)
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

    private async Task ValidarUsuario(Usuario usuario, int usuarioId)
    {
        var erros = new List<ValidationError>();

        if (usuario.UnidadeOrganizacionalId <= 0)
            erros.Add(new ValidationError("UnidadeOrganizacional", "Informe a unidade organizacional."));

        if (string.IsNullOrWhiteSpace(usuario.Username))
        {
            erros.Add(new ValidationError(nameof(usuario.Username), "Informe o username."));
        }
        else
        {
            bool usernameExiste = await _repository.VerificarUsuarioExiste(usuario.Username, usuario.UnidadeOrganizacionalId, usuarioId);

            if (usernameExiste)
                erros.Add(new ValidationError(nameof(usuario.Username), "Este username já está sendo usado por outro usuário."));
        }

        if (string.IsNullOrWhiteSpace(usuario.Senha))
            erros.Add(new ValidationError("Senha", "Informe a senha."));

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            erros.Add(new ValidationError("Nome", "Informe o nome."));

        if (string.IsNullOrWhiteSpace(usuario.Telefone))
            erros.Add(new ValidationError("Telefone", "Informe o telefone."));

        if (erros.Any())
            throw new ValidationException(erros);
    }
}