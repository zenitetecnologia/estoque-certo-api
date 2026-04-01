using Estoque.models;
using Estoque.Repositories;
using Estoque.Server.Services;
using Estoque.Server.Validations;

namespace Estoque.Services;

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
            var espacoExistente = await _repository.ObterEspaco(espacoId);
            if (espacoExistente == null) throw new NotFoundException("Espaço não encontrado para o ID informado.");

            espaco.UnidadeOrganizacionalId = espacoExistente.UnidadeOrganizacionalId;

            await ValidarEspaco(espaco, espacoId);

            return await _repository.AtualizarEspaco(espaco, espacoId);
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
            throw new Exception($"Erro ao excluir espaço: {ex.Message}");
        }
    }

    public async Task<List<EspacoRecuperado>> ObterEspacos()
    {
        try
        {
            return await _repository.ObterEspacos();
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