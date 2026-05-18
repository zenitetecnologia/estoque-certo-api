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

    public async Task<Guid> Cadastrar(ItemEstoque item)
    {
        try
        {
            await ValidarCampos(item, Guid.Empty);

            Guid itemId = await _repository.Cadastrar(item);

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

    public async Task<int> Atualizar(ItemEstoque item, Guid itemEstoqueId)
    {
        try
        {
            var itemExistente = await _repository.Obter(itemEstoqueId);

            if (itemExistente == null) throw new NotFoundException("Item de estoque não encontrado com os parâmetros informados.");

            item.UnidadeOrganizacionalId = itemExistente.UnidadeOrganizacionalId;

            await ValidarCampos(item, itemEstoqueId);

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


    public async Task<List<ItemEstoqueRecuperado>> Obter(int skip, int top, string? descricao, Guid? unidadeOrganizacionalId, Guid? espacoId)
    {
        try
        {
            return await _repository.Obter(skip, top, descricao, unidadeOrganizacionalId, espacoId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao Obter itens: {ex.Message}");
        }
    }

    public async Task<ItemEstoqueRecuperado> Obter(Guid itemEstoqueId)
    {
        try
        {
            var item = await _repository.Obter(itemEstoqueId);

            if (item == null) throw new NotFoundException("Item não encontrado com o ID informado.");

            return item;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao obter item por ID: {ex.Message}");
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

    public async Task<bool> Movimentar(Guid itemEstoqueId, decimal quantidade, TipoMovimentacao tipo, Guid? usuarioId)
    {
        try
        {
            if (quantidade <= 0)
            {
                AddError("Quantidade", "A quantidade de movimentação deve ser maior que zero.");
                throw new ValidationException(Errors);
            }

            var sucesso = await _repository.Movimentar(itemEstoqueId, quantidade, tipo, usuarioId);

            if (!sucesso)
            {
                if (tipo == TipoMovimentacao.Subtracao)
                    throw new InvalidOperationException("Estoque insuficiente para realizar esta saída, ou item não encontrado.");
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

    public async Task<int> Excluir(Guid itemEstoqueId)
    {
        try
        {
            var affected = await _repository.Excluir(itemEstoqueId);

            if (affected <= 0) throw new NotFoundException("Item não encontrado com o ID informado.");

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

    public async Task<bool> Transferir(Guid itemEstoqueId, Guid novoEspacoId)
    {
        try
        {
            if (novoEspacoId == Guid.Empty)
            {
                AddError("NovoEspacoId", "Informe o novo espaço de destino.");
                throw new ValidationException(Errors);
            }

            var itemExistente = await _repository.Obter(itemEstoqueId);

            if (itemExistente == null)
                throw new NotFoundException("Item de estoque não encontrado.");

            if (itemExistente.EspacoId == novoEspacoId)
            {
                AddError("NovoEspacoId", "O item já se encontra neste espaço.");
                throw new ValidationException(Errors);
            }

            var affected = await _repository.TransferirEspaco(itemEstoqueId, novoEspacoId);

            return affected > 0;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao transferir item: {ex.Message}");
        }
    }

    private async Task ValidarCampos(ItemEstoque item, Guid itemEstoqueId)
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