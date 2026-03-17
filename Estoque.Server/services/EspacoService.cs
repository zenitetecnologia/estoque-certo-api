using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IEspacosService
{
    Task<List<Espacos>> ObterTodosAsync();
    Task<Espacos?> ObterPorIdAsync(int id);
    Task<int> CadastrarAsync(Espacos espaco);
    Task<bool> AtualizarAsync(Espacos espaco);
    Task<bool> ExcluirAsync(int id);
}

public class EspacosService : IEspacosService
{
    private readonly EspacosRepository _repository;

    public EspacosService(EspacosRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Espacos>> ObterTodosAsync() => await _repository.ObterEspacos();

    public async Task<Espacos?> ObterPorIdAsync(int id) => await _repository.ObterEspacoPorId(id);

    public async Task<int> CadastrarAsync(Espacos espaco)
    {
        if (string.IsNullOrWhiteSpace(espaco.Nome)) throw new ArgumentException("O nome do espaço é obrigatório.");
        if (espaco.IdUnidadeOrganizacional <= 0) throw new ArgumentException("A Unidade Organizacional é obrigatória.");
        return await _repository.CadastrarEspaco(espaco);
    }

    public async Task<bool> AtualizarAsync(Espacos espaco)
    {
        if (string.IsNullOrWhiteSpace(espaco.Nome)) throw new ArgumentException("O nome do espaço é obrigatório.");
        return await _repository.AtualizarEspaco(espaco);
    }

    public async Task<bool> ExcluirAsync(int id) => await _repository.ExcluirEspaco(id);
}