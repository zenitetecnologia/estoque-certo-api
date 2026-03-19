using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IUsuarioService
{
    Task<List<Usuario>> ListarTodosAsync();
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<int> RegistarUsuarioNormalAsync(Usuario usuario);
    Task<bool> AtualizarAsync(Usuario usuario);
    Task<bool> ExcluirAsync(int id);
}

public class UsuarioService : IUsuarioService
{
    private readonly UsuarioRepository _repository;

    public UsuarioService(UsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Usuario>> ListarTodosAsync()
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

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _repository.ObterUsuarioPorId(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar usuário por ID: {ex.Message}");
        }
    }

    public async Task<int> RegistarUsuarioNormalAsync(Usuario usuario)
    {
        try
        {
            usuario.Perfil = PerfilUsuario.Normal;
            usuario.Validado = false;

            if (string.IsNullOrWhiteSpace(usuario.Username) || string.IsNullOrWhiteSpace(usuario.Senha))
            {
                throw new ArgumentException("Username e Senha são obrigatórios.");
            }

            bool usernameExiste = await _repository.VerificaExisteUsuario(usuario.Username, 0);
            if (usernameExiste)
            {
                throw new InvalidOperationException("Este Username já está em uso.");
            }

            return await _repository.CadastrarUsuario(usuario);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registar usuário normal: {ex.Message}");
        }
    }

    public async Task<bool> AtualizarAsync(Usuario usuario)
    {
        try
        {
            bool usernameExiste = await _repository.VerificaExisteUsuario(usuario.Username, usuario.Id);
            if (usernameExiste)
            {
                throw new InvalidOperationException("Este Username já está a ser usado por outro usuário.");
            }

            return await _repository.AtualizarUsuario(usuario);
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
            return await _repository.ExcluirUsuario(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir usuário: {ex.Message}");
        }
    }
}