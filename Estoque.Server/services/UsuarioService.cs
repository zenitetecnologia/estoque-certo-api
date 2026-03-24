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

    public async Task<int> CriarUsuario(Usuario usuario)
    {
        try
        {
            await ValidarUsuario(usuario, 0);

            usuario.Perfil = PerfilUsuario.Normal;

            int novoIdUsuario = await _repository.CadastrarUsuario(usuario);

            return novoIdUsuario;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registar usuário: {ex.Message}");
        }
    }

























    public async Task<List<UsuarioRecuperado>> ListarTodosAsync()
    {
        try
        {
            return await _repository.ObterUsuarios();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar todos usuários: {ex.Message}");
        }
    }

    public async Task<UsuarioRecuperado?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _repository.ObterUsuario(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar usuário por ID: {ex.Message}");
        }
    }



    public async Task<bool> AtualizarUsuario(Usuario usuario, int usuarioId)
    {
        try
        {
            await ValidarUsuario(usuario, usuarioId);

            bool usernameExiste = await _repository.VerificaExisteUsuario(usuario.Username, usuarioId);

            if (usernameExiste)
            {
                throw new InvalidOperationException("Este Username já está sendo usado por outro usuário.");
            }

            bool atualizado = await _repository.AtualizarUsuario(usuario, usuarioId);

            if (usernameExiste)
            {
                throw new InvalidOperationException("Este Username já está sendo usado por outro usuário.");
            }

            return atualizado;
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

    public async Task<bool> ExcluirAsync(int id)
    {
        try
        {
            var usuario = await _repository.ObterUsuario(id);

            if (usuario == null)
            {
                throw new InvalidOperationException("Nenhum usuário encontrado com esse id");
            }

            return await _repository.ExcluirUsuario(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir usuário: {ex.Message}");
        }
    }

    public async Task VincularComUnidades(UsuarioValido request)
    {
        var usuario = await _repository.ObterUsuario(request.UsuarioId);

        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }

        await _repository.RemoverUnidadesOrganizacionais(request.UsuarioId);

        foreach (var unidade in request.UnidadesOrganizacionais)
        {
            await _repository.VincularUnidadeOrganizacional(request.UsuarioId, unidade.UnidadeOrganizacionalId);
        }
    }

    private async Task ValidarUsuario(Usuario usuario, int usuarioId)
    {
        var erros = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(usuario.Username))
        {
            erros.Add(new ValidationError(nameof(usuario.Username), "Informe o username."));
        }
        else
        {
            bool usernameExiste = await _repository.VerificaExisteUsuario(usuario.Username, usuarioId);

            if (usernameExiste)
                erros.Add(new ValidationError(nameof(usuario.Username), "Este username já está sendo usado por outro usuário."));
        }

        if (string.IsNullOrWhiteSpace(usuario.Senha))
            erros.Add(new ValidationError("Senha", "Informe a senha."));

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            erros.Add(new ValidationError("Nome", "Informe o nome."));

        if (string.IsNullOrWhiteSpace(usuario.Telefone))
            erros.Add(new ValidationError("Telefone", "Informe o telefone."));

        if (usuario.UnidadesOrganizacionais == null || !usuario.UnidadesOrganizacionais.Any())
            erros.Add(new ValidationError(nameof(usuario.UnidadesOrganizacionais), "Informe pelo menos uma unidade organizacional."));

        if (erros.Any())
            throw new ValidationException(erros);



    }
}