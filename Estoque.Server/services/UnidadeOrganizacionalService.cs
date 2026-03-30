using Estoque.models;
using Estoque.Repositories;
using Estoque.Server.Services;
using Estoque.Server.Validations;

namespace Estoque.Services;

public class UnidadeOrganizacionalService : BaseService
{
    private readonly UnidadeOrganizacionalRepository _repository;

    public UnidadeOrganizacionalService(UnidadeOrganizacionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> CriarUnidade(UnidadeOrganizacional unidade)
    {
        try
        {
            await ValidarUnidadeOrganizacional(unidade, 0);

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

    public async Task<bool> AtualizarUnidade(UnidadeOrganizacional unidade, int unidadeOrganizacionalId)
    {
        try
        {
            var unidadeorganizacional = await _repository.ObterUnidade(unidadeOrganizacionalId);

            if (unidadeorganizacional == null) throw new NotFoundException("Unidade organizacional não encontrada para o ID informado.");

            await ValidarUnidadeOrganizacional(unidade, unidadeOrganizacionalId);

            return await _repository.AtualizarUnidade(unidade, unidadeOrganizacionalId);
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

    public async Task<List<UnidadeOrganizacional>> ObterUnidades()
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

    public async Task<UnidadeOrganizacional> ObterUnidadePorId(int unidadeOrganizacionalId)
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

    public async Task<bool> ExcluirUnidade(int unidadeOrganizacionalId)
    {
        try
        {
            var unidadeExistente = await _repository.ObterUnidade(unidadeOrganizacionalId);

            if (unidadeExistente == null)
            {
                throw new NotFoundException("Unidade organizacional não encontrada para o id informado.");
            }

            return await _repository.ExcluirUnidade(unidadeOrganizacionalId);
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

    private async Task ValidarUnidadeOrganizacional(UnidadeOrganizacional unidade, int unidadeOrganizacionalId)
    {

        if (string.IsNullOrWhiteSpace(unidade.RazaoSocial))
            AddError(nameof(unidade.RazaoSocial), "Informe a razão social.");

        if (!string.IsNullOrWhiteSpace(unidade.Cnpj))
        {
            if (!RulleValidation.CnpjValido(unidade.Cnpj))
            {
                //        _errors.Add(new ValidationError("Cnpj", "O CNPJ informado não é válido."));
                //    }
                //    else
                //    {
                //        bool cnpjExiste = await _repository.VerificaExisteUnidade(unidade.Cnpj, unidadeOrganizacionalId);
                //        if (cnpjExiste)
                //        {
                //            _errors.Add(new ValidationError("Cnpj", "Este CNPJ já está cadastrado em outra unidade."));
                //        }
                //    }
                //}

                //if (_errors.Any())
                //    throw new ValidationException(_errors);
            }
        }
    }
}