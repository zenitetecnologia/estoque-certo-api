using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IItemEstoqueService
{
    Task<List<ItemEstoque>> ObterTodosAsync();
    Task<ItemEstoque?> ObterPorIdAsync(int id);
    Task<int> CadastrarAsync(ItemEstoque item);
    Task<bool> AtualizarAsync(ItemEstoque item);
    Task<bool> ExcluirAsync(int id);
}

public class ItemEstoqueService : IItemEstoqueService
{
    private readonly ItemEstoqueRepository _repository;

    public ItemEstoqueService(ItemEstoqueRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ItemEstoque>> ObterTodosAsync()
    {
        return await _repository.ObterItensEstoque();
    }

    public async Task<ItemEstoque?> ObterPorIdAsync(int id)
    {
        return await _repository.ObterItemEstoquePorId(id);
    }

    public async Task<int> CadastrarAsync(ItemEstoque item)
    {
        if (string.IsNullOrWhiteSpace(item.Descricao))
        {
            throw new ArgumentException("A descrição do item é obrigatória.");
        }

        if (item.Quantidade < 0)
        {
            throw new ArgumentException("A quantidade inicial não pode ser negativa.");
        }

        return await _repository.CadastrarItemEstoque(item);
    }

    public async Task<bool> AtualizarAsync(ItemEstoque item)
    {
        if (string.IsNullOrWhiteSpace(item.Descricao))
        {
            throw new ArgumentException("A descrição do item é obrigatória.");
        }

        return await _repository.AtualizarItemEstoque(item);
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        return await _repository.ExcluirItemEstoque(id);
    }
}