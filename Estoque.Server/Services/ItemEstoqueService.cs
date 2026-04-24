using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;

namespace Estoque.Server.Services;

public class ItemEstoqueService : BaseService
{
    private readonly ItemEstoqueRepository _repository;
    private readonly HistoricoRepository _historicoRepository;

    public ItemEstoqueService(ItemEstoqueRepository repository, HistoricoRepository historicoRepository)
    {
        _repository = repository;
        _historicoRepository = historicoRepository;
    }

    public async Task<Guid> CadastrarItemEstoque(ItemEstoque item)
    {
        try
        {
            await ValidarItemEstoque(item, Guid.Empty);

            Guid itemId = await _repository.CadastrarItemEstoque(item);

            return itemId;
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

            if (itemExistente == null) throw new NotFoundException("Item de estoque não encontrado com os parâmetros informados.");

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


    public async Task<List<ItemEstoqueRecuperado>> ObterItens(int skip, int top, string? descricao, Guid? unidadeOrganizacionalId, Guid? espacoId)
    {
        try
        {
            return await _repository.ObterItens(skip, top, descricao, unidadeOrganizacionalId, espacoId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar os itens: {ex.Message}");
        }
    }

    public async Task<ItemEstoqueRecuperado> ObterItem(Guid itemEstoqueId)
    {
        try
        {
            var item = await _repository.ObterItem(itemEstoqueId);

            if (item == null) throw new NotFoundException("Item não encontrado para o ID informado.");

            return item;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar item por ID: {ex.Message}");
        }
    }

    public async Task<List<HistoricoRecuperado>> ObterHistorico(Guid itemEstoqueId)
    {
        try
        {
            return await _historicoRepository.ObterHistoricoPorItem(itemEstoqueId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar  histórico do item: {ex.Message}");
        }
    }

    public async Task<bool> MovimentarEstoque(Guid itemEstoqueId, decimal quantidade, TipoMovimentacao tipo, Guid? usuarioId)
    {
        try
        {
            if (quantidade <= 0)
            {
                AddError("Quantidade", "A quantidade de movimentação deve ser maior que zero.");
                throw new ValidationException(Errors);
            }

            var sucesso = await _repository.MovimentarEstoque(itemEstoqueId, quantidade, tipo, usuarioId);

            if (!sucesso)
            {
                if (tipo == TipoMovimentacao.Subtracao)
                    throw new InvalidOperationException("Estoque insuficiente para realizar esta saída ou item não encontrado.");
                else
                    throw new NotFoundException("Item não encontrado.");
            }

            return true;
        }
        catch (InvalidOperationException)
        {
            throw;
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
            throw new Exception($"Erro ao movimentar estoque: {ex.Message}");
        }
    }

    public async Task<int> ExcluirItemEstoque(Guid itemEstoqueId)
    {
        try
        {
            var affected = await _repository.ExcluirItemEstoque(itemEstoqueId);

            if (affected <= 0) throw new NotFoundException("Item não encontrado para o ID informado.");

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

    private async Task ValidarItemEstoque(ItemEstoque item, Guid itemEstoqueId)
    {
        if (itemEstoqueId == Guid.Empty && item.UnidadeOrganizacionalId == Guid.Empty)
            AddError(nameof(item.UnidadeOrganizacionalId), "Informe a unidade organizacional.");

        if (item.EspacoId == Guid.Empty)
            AddError(nameof(item.EspacoId), "Informe um espaço para o item.");

        if (string.IsNullOrWhiteSpace(item.Descricao))
            AddError(nameof(item.Descricao), "Informe a descrição do item.");

        if (item.Quantidade < 0)
            AddError(nameof(item.Quantidade), "Informe a quantidade.");

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}