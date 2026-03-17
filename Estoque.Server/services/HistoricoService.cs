using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IHistoricoService
{
    Task<List<Historico>> ObterHistoricoPorItemAsync(int idItemEstoque);
    Task<int> RegistarMovimentacaoAsync(Historico historico);
}

public class HistoricoService : IHistoricoService
{
    private readonly HistoricoRepository _repository;

    public HistoricoService(HistoricoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Historico>> ObterHistoricoPorItemAsync(int idItemEstoque)
    {
        return await _repository.ObterHistoricoPorItem(idItemEstoque);
    }

    public async Task<int> RegistarMovimentacaoAsync(Historico historico)
    {
        historico.DataHora = DateTime.Now;
        return await _repository.RegistarHistorico(historico);
    }
}