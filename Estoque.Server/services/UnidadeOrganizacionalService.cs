using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;

public interface IUnidadeOrganizacionalService
{
    Task<List<UnidadeOrganizacional>> ObterTodasAsync();
    Task<UnidadeOrganizacional?> ObterPorIdAsync(int id);
    Task<int> CadastrarAsync(UnidadeOrganizacional unidade);
    Task<bool> AtualizarAsync(UnidadeOrganizacional unidade);
    Task<bool> ExcluirAsync(int id);
}

public class UnidadeOrganizacionalService : IUnidadeOrganizacionalService
{
    private readonly UnidadeOrganizacionalRepository _repository;

    public UnidadeOrganizacionalService(UnidadeOrganizacionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UnidadeOrganizacional>> ObterTodasAsync()
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

    public async Task<UnidadeOrganizacional?> ObterPorIdAsync(int id)
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

    public async Task<int> CadastrarAsync(UnidadeOrganizacional unidade)
    {
        try
        {
            return await _repository.CadastrarUnidade(unidade);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar unidade organizacional: {ex.Message}");
        }
    }

    public async Task<bool> AtualizarAsync(UnidadeOrganizacional unidade)
    {
        try
        {
            return await _repository.AtualizarUnidade(unidade);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar unidade organizacional: {ex.Message}");
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
}