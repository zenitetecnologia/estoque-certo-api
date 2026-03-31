using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;


public class ItemEstoqueService
{
    private readonly ItemEstoqueRepository _repository;

    public ItemEstoqueService(ItemEstoqueRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ItemEstoque>> ObterItens()
    {
        try
        {
            return await _repository.ObterItens();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar todos os itens: {ex.Message}");
        }
    }

    public async Task<ItemEstoque?> ObterItem(int itemEstoqueId)
    {
        try
        {
            return await _repository.ObterItem(itemEstoqueId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar item por ID: {ex.Message}");
        }
    }

    public async Task<int> CadastrarItemEstoque(ItemEstoque item)
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

    public async Task<bool> AtualizarItemEstoque(ItemEstoque item)
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

    public async Task<bool> ExcluirItemEstoque(int itemEstoqueId)
    {
        try
        {
            return await _repository.ExcluirItemEstoque(itemEstoqueId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir item: {ex.Message}");
        }
    }

    private async Task ValidarItemEstoque(ItemEstoque itemEstoque, int itemEstoqueId)
    {

    }
}