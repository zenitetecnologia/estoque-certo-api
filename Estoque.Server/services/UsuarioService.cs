using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IUsuarioService
{
    Task<List<Usuario>> ListarTodosAsync();
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<int> RegistarUsuarioNormalAsync(Usuario usuario);
    Task AprovarUsuarioAsync(int idUsuarioAdmin, int idUsuarioParaAprovar);
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
        return await _repository.ObterUsuarios();
    }

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        return await _repository.ObterUsuarioPorId(id);
    }

    public async Task<int> RegistarUsuarioNormalAsync(Usuario usuario)
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

    public async Task AprovarUsuarioAsync(int idUsuarioAdmin, int idUsuarioParaAprovar)
    {
        var admin = await _repository.ObterUsuarioPorId(idUsuarioAdmin)
                    ?? throw new UnauthorizedAccessException("Administrador não encontrado.");

        if (admin.Perfil != PerfilUsuario.Admin)
        {
            throw new UnauthorizedAccessException("Apenas administradores podem aprovar acessos.");
        }

        var usuarioPendente = await _repository.ObterUsuarioPorId(idUsuarioParaAprovar)
                              ?? throw new KeyNotFoundException("Utilizador não encontrado.");

        if (usuarioPendente.Validado)
        {
            throw new InvalidOperationException("Este utilizador já está validado.");
        }

        usuarioPendente.Validado = true;
        await _repository.AtualizarUsuario(usuarioPendente);
    }

    public async Task<bool> AtualizarAsync(Usuario usuario)
    {
        bool usernameExiste = await _repository.VerificaExisteUsuario(usuario.Username, usuario.Id);
        if (usernameExiste)
        {
            throw new InvalidOperationException("Este Username já está a ser usado por outro utilizador.");
        }

        return await _repository.AtualizarUsuario(usuario);
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        return await _repository.ExcluirUsuario(id);
    }
}