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
        try
        {
            return await _repository.ObterItensEstoque();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar todos os itens: {ex.Message}");
        }
    }

    public async Task<ItemEstoque?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _repository.ObterItemEstoquePorId(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar item por ID: {ex.Message}");
        }
    }

    public async Task<int> CadastrarAsync(ItemEstoque item)
    {
        try
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
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar item de estoque: {ex.Message}");
        }
    }

    public async Task<bool> AtualizarAsync(ItemEstoque item)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item.Descricao))
            {
                throw new ArgumentException("A descrição do item é obrigatória.");
            }

            return await _repository.AtualizarItemEstoque(item);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar item de estoque: {ex.Message}");
        }
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        try
        {
            return await _repository.ExcluirItemEstoque(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir item: {ex.Message}");
        }
    }
}