using Estoque.Models;
using Estoque.Repositories;
using Estoque.Server.Services;
using Estoque.Server.Validations;

namespace Estoque.Services;

public class ItemEstoqueService : BaseService
{
    private readonly ItemEstoqueRepository _repository;

    public ItemEstoqueService(ItemEstoqueRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CadastrarItemEstoque(ItemEstoque item)
    {
        try
        {
            await ValidarItemEstoque(item, Guid.Empty);

            return await _repository.CadastrarItemEstoque(item);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar item de estoque: {ex.Message}");
        }
    }

    public async Task<int> AtualizarItemEstoque(ItemEstoque item, Guid itemEstoqueId)
    {
        try
        {
            var itemExistente = await _repository.ObterItem(itemEstoqueId);

            if (itemExistente == null) throw new NotFoundException("Item de estoque não encontrado para o ID informado.");

            item.UnidadeOrganizacionalId = itemExistente.UnidadeOrganizacionalId;

            await ValidarItemEstoque(item, itemEstoqueId);

            return await _repository.AtualizarItemEstoque(item, itemEstoqueId);
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
            throw new Exception($"Erro ao atualizar item de estoque: {ex.Message}");
        }
    }

    public async Task<int> ExcluirItemEstoque(Guid itemEstoqueId)
    {
        try
        {
            var affected = await _repository.ExcluirItemEstoque(itemEstoqueId);

            if (affected <= 0) throw new NotFoundException("Item de estoque não encontrado para o ID informado.");

            return affected;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir item: {ex.Message}");
        }
    }

    public async Task<List<ItemEstoqueRecuperado>> ObterItens()
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

    public async Task<ItemEstoqueRecuperado> ObterItem(Guid itemEstoqueId)
    {
        try
        {
            var item = await _repository.ObterItem(itemEstoqueId);

            if (item == null) throw new NotFoundException("Item de estoque não encontrado para o ID informado.");

            return item;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar item por ID: {ex.Message}");
        }
    }

    private async Task ValidarItemEstoque(ItemEstoque item, Guid itemEstoqueId)
    {
        if (itemEstoqueId == Guid.Empty && item.UnidadeOrganizacionalId == Guid.Empty)
            AddError(nameof(item.UnidadeOrganizacionalId), "Informe a unidade organizacional.");

        if (item.Espaco <= 0)
            AddError(nameof(item.Espaco), "Informe um espaço para o item.");

        if (string.IsNullOrWhiteSpace(item.Descricao))
            AddError(nameof(item.Descricao), "Informe a descrição do item.");

        if (item.Quantidade < 0)
            AddError(nameof(item.Quantidade), "Informe a quantidade.");

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}