using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IHistoricoService
{
    Task<List<Historico>> ObterHistoricoPorItemAsync(int idItem);
    Task<int> RegistarMovimentacaoAsync(Historico historico);
}

public class HistoricoService : IHistoricoService
{
    private readonly HistoricoRepository _repository;

    public HistoricoService(HistoricoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Historico>> ObterHistoricoPorItemAsync(int idItem)
    {
        try
        {
            return await _repository.ObterHistoricoPorItem(idItem);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar o histórico do item: {ex.Message}");
        }
    }

    public async Task<int> RegistarMovimentacaoAsync(Historico historico)
    {
        try
        {
            return await _repository.RegistarHistorico(historico);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registar movimentação no histórico: {ex.Message}");
        }
    }
}