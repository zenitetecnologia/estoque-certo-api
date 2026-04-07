using Estoque.Server.Exceptions;
using Estoque.Server.Models;
using Estoque.Server.Repositories;
using Estoque.Server.Validations;

namespace Estoque.Server.Services;

public class UnidadeOrganizacionalService : BaseService
{
    private readonly UnidadeOrganizacionalRepository _repository;

    public UnidadeOrganizacionalService(UnidadeOrganizacionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CriarUnidade(UnidadeOrganizacional unidade)
    {
        try
        {
            await ValidarUnidadeOrganizacional(unidade, Guid.Empty);

            return await _repository.CriarUnidade(unidade);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar unidade: {ex.Message}");
        }
    }

    public async Task<int> AtualizarUnidade(UnidadeOrganizacional unidade, Guid unidadeOrganizacionalId)
    {
        try
        {
            await ValidarUnidadeOrganizacional(unidade, unidadeOrganizacionalId);

            var affected = await _repository.AtualizarUnidade(unidade, unidadeOrganizacionalId);

            if (affected <= 0) throw new NotFoundException("Unidade organizacional não encontrada para o ID informado.");

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
            throw new Exception($"Erro ao atualizar unidade organizacional: {ex.Message}");
        }
    }

    public async Task<List<UnidadeOrganizacionalRecuperado>> ObterUnidades()
    {
        try
        {
            return await _repository.ObterUnidades();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar todas as unidades organizacionais: {ex.Message}");
        }
    }

    public async Task<UnidadeOrganizacionalRecuperado> ObterUnidadePorId(Guid unidadeOrganizacionalId)
    {
        try
        {
            var unidade = await _repository.ObterUnidade(unidadeOrganizacionalId);

            if (unidade == null)
            {
                throw new NotFoundException("Unidade organizacional não encontrada para o id informado.");
            }

            return unidade;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar unidade organizacional por ID: {ex.Message}");
        }
    }

    public async Task<int> ExcluirUnidade(Guid unidadeOrganizacionalId)
    {
        try
        {
            var affected = await _repository.ExcluirUnidade(unidadeOrganizacionalId);

            if (affected <= 0) throw new NotFoundException("Unidade organizacional não encontrada para o ID informado.");

            return affected;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir unidade organizacional: {ex.Message}");
        }
    }

    private async Task ValidarUnidadeOrganizacional(UnidadeOrganizacional unidade, Guid unidadeOrganizacionalId)
    {

        if (string.IsNullOrWhiteSpace(unidade.RazaoSocial))
            AddError(nameof(unidade.RazaoSocial), "Informe a razão social.");

        if (!string.IsNullOrWhiteSpace(unidade.Cnpj))
        {
            if (!RuleValidation.CnpjValido(unidade.Cnpj))
            {
                AddError(nameof(unidade.Cnpj), "o cnpj informado não é válido.");
            }
            else
            {
                bool cnpjexiste = await _repository.VerificaExisteUnidade(unidade.Cnpj, unidadeOrganizacionalId);
                if (cnpjexiste)
                {
                    AddError(nameof(unidade.Cnpj), "este cnpj já está cadastrado em outra unidade.");
                }
            }
        }

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}