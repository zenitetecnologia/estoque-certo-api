using cantinou.Server.Validations;
using Estoque.models;
using Estoque.Repositories;
using Estoque.Server.Validations;

namespace Estoque.Services;


public class UnidadeOrganizacionalService
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

            return await _repository.CadastrarUnidade(unidade);
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
















    public async Task<List<UnidadeOrganizacionalRecuperado>> ObterTodasAsync()
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

    public async Task<UnidadeOrganizacionalRecuperado?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _repository.ObterUnidadePorId(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar unidade organizacional por ID: {ex.Message}");
        }
    }



    public async Task<bool> ExcluirAsync(int id)
    {
        try
        {
            return await _repository.ExcluirUnidade(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir unidade organizacional: {ex.Message}");
        }
    }

    private async Task ValidarUnidadeOrganizacional(UnidadeOrganizacional unidade, int unidadeId)
    {
        var erros = new List<ValidationError>();

        if (unidadeId > 0)
        {
            var unidadeExistente = await _repository.ObterUnidadePorId(unidadeId);
            if (unidadeExistente == null)
            {
                erros.Add(new ValidationError("id", "Nenhuma unidade organizacional encontrada com esse ID"));
            }
        }

        if (string.IsNullOrWhiteSpace(unidade.RazaoSocial))
            erros.Add(new ValidationError("RazaoSocial", "Informe a razão social."));

        if (!string.IsNullOrWhiteSpace(unidade.Cnpj))
        {
            if (!RulleValidation.IsCnpjValido(unidade.Cnpj))
            {
                erros.Add(new ValidationError("Cnpj", "O CNPJ informado não é válido."));
            }
            else
            {
                bool cnpjExiste = await _repository.VerificaExisteUnidade(unidade.Cnpj, unidadeId);
                if (cnpjExiste)
                {
                    erros.Add(new ValidationError("Cnpj", "Este CNPJ já está cadastrado em outra unidade."));
                }
            }
        }

        if (string.IsNullOrWhiteSpace(unidade.Email))
            erros.Add(new ValidationError("Email", "Informe o email."));

        if (erros.Any())
            throw new ValidationException(erros);
    }
}