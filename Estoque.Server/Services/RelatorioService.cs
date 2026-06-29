using Estoque.Server.Models;
using Estoque.Server.Repositories;

namespace Estoque.Server.Services;

public class RelatorioService : BaseService
{
    private readonly RelatorioRepository _repository;

    public RelatorioService(RelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PizzaDashboardItem>> ObterPizzaDashboard(
        Guid unidadeOrganizacionalId,
        Guid? espacoId,
        int? tipoUnidadeMedida
    )
    {
        try
        {
            // Aqui você poderia colocar validações específicas de relatório,
            // por exemplo checar se unidadeOrganizacionalId != Guid.Empty, etc.

            return await _repository.ObterPizzaDashboard(
                unidadeOrganizacionalId,
                espacoId,
                tipoUnidadeMedida
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao obter dados do dashboard de pizza: {ex.Message}");
        }
    }
}