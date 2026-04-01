using Estoque.Models;
using Estoque.Repositories;

namespace Estoque.Services;


public class HistoricoService
{
    private readonly HistoricoRepository _repository;

    public HistoricoService(HistoricoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Historico>> ObterHistoricoPorItem(int idItem)
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

    public async Task<int> RegistrarHistorico(Historico historico)
    {
        try
        {
            return await _repository.RegistrarHistorico(historico);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registar movimentação no histórico: {ex.Message}");
        }
    }
}