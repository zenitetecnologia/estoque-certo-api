using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;

namespace Estoque.Server.Services;

public class EspacoService : BaseService
{
    private readonly EspacoRepository _repository;

    public EspacoService(EspacoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CadastrarEspaco(Espaco espaco)
    {
        try
        {
            await ValidarEspaco(espaco, Guid.Empty);

            return await _repository.CadastrarEspaco(espaco);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar espaço: {ex.Message}");
        }
    }

    public async Task<int> AtualizarEspaco(Espaco espaco, Guid espacoId)
    {
        try
        {
            await ValidarEspaco(espaco, espacoId);

            var affected = await _repository.AtualizarEspaco(espaco, espacoId);

            if (affected <= 0) throw new NotFoundException("Espaço não encontrado para o ID informado.");

            return affected;
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
            throw new Exception($"Erro ao atualizar espaço: {ex.Message}");
        }
    }

    public async Task<int> ExcluirEspaco(Guid espacoId)
    {
        try
        {
            var affected = await _repository.ExcluirEspaco(espacoId);

            if (affected <= 0) throw new NotFoundException("Espaço não encontrado para o ID informado.");

            return affected;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("fk_item_espaco") || ex.InnerException?.Message.Contains("fk_item_espaco") == true)
            {
                AddError("EspacoId", "Não é possível excluir este espaço porque ainda existem itens de estoque armazenados nele. Mova ou exclua os itens primeiro.");
                throw new ValidationException(Errors);
            }
            throw new Exception($"Erro ao excluir espaço: {ex.Message}");
        }
    }

    public async Task<List<EspacoRecuperado>> ObterEspacos(int skip, int top, string? nome, Guid? unidadeOrganizacionalId)
    {
        try
        {
            return await _repository.ObterEspacos(skip, top, nome, unidadeOrganizacionalId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar espaços: {ex.Message}");
        }
    }

    public async Task<EspacoRecuperado> ObterEspaco(Guid espacoId)
    {
        try
        {
            var espaco = await _repository.ObterEspaco(espacoId);

            if (espaco == null) throw new NotFoundException("Espaço não encontrado para o ID informado.");

            return espaco;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar espaço por ID: {ex.Message}");
        }
    }

    private async Task ValidarEspaco(Espaco espaco, Guid espacoId)
    {
        if (espacoId == Guid.Empty && espaco.UnidadeOrganizacionalId == Guid.Empty)
            AddError(nameof(espaco.UnidadeOrganizacionalId), "A Unidade Organizacional é obrigatória.");

        if (string.IsNullOrWhiteSpace(espaco.Nome))
            AddError(nameof(espaco.Nome), "Informe o nome do espaço");

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}