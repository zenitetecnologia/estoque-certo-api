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

    public async Task<Guid> CadastrarUnidade(UnidadeOrganizacional unidade)
    {
        try
        {
            await ValidarUnidadeOrganizacional(unidade, Guid.Empty);

            return await _repository.CadastrarUnidade(unidade);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar unidade organizacional: {ex.Message}");
        }
    }

    public async Task<int> AtualizarUnidade(UnidadeOrganizacional unidade, Guid unidadeOrganizacionalId)
    {
        try
        {
            await ValidarUnidadeOrganizacional(unidade, unidadeOrganizacionalId);

            var affected = await _repository.AtualizarUnidade(unidade, unidadeOrganizacionalId);

            if (affected <= 0) throw new NotFoundException("Unidade organizacional não encontrada com os parâmetros informados.");

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

    public async Task<List<UnidadeOrganizacionalGetResponse>> ObterUnidades(int skip, int top, string? razaoSocial, string? Cnpj)
    {
        try
        {
            return await _repository.ObterUnidades(skip, top, razaoSocial, Cnpj);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar unidades organizacionais: {ex.Message}");
        }
    }

    public async Task<UnidadeOrganizacionalGetResponse> ObterUnidade(Guid unidadeOrganizacionalId)
    {
        try
        {
            var unidade = await _repository.ObterUnidade(unidadeOrganizacionalId);

            if (unidade == null) throw new NotFoundException("Unidade organizacional não encontrada para o id informado.");

            return unidade;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar unidade organizacional por ID: {ex.Message}");
        }
    }

    private async Task ValidarUnidadeOrganizacional(UnidadeOrganizacional unidade, Guid unidadeOrganizacionalId)
    {

        if (string.IsNullOrWhiteSpace(unidade.RazaoSocial))
            AddError(nameof(unidade.RazaoSocial), "Informe a razão social.");

        if (string.IsNullOrWhiteSpace(unidade.Cnpj))
        {
            AddError(nameof(unidade.Cnpj), "Informe o CNPJ.");
        }
        else
        {
            if (!RuleValidation.CnpjValido(unidade.Cnpj))
            {
                AddError(nameof(unidade.Cnpj), "CNPJ inválido.");
            }
            else
            {
                bool cnpjExiste = await _repository.VerificaUnidadeExiste(unidade.Cnpj, unidadeOrganizacionalId);

                if (cnpjExiste)
                    AddError(nameof(unidade.Cnpj), "Este CNPJ já está cadastrado em outra unidade organizacional.");
            }
        }

        if (Errors.Any())
            throw new ValidationException(Errors);
    }
}