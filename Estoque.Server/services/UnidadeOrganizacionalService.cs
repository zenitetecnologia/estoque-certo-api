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

    public async Task<List<UnidadeOrganizacional>> ObterTodasAsync() => await _repository.ObterUnidades();

    public async Task<UnidadeOrganizacional?> ObterPorIdAsync(int id) => await _repository.ObterUnidadePorId(id);

    public async Task<int> CadastrarAsync(UnidadeOrganizacional unidade)
    {
        if (!string.IsNullOrWhiteSpace(unidade.Cpnj))
        {
            var existe = await _repository.VerificaExisteUnidade(unidade.Cpnj, 0);
            if (existe) throw new InvalidOperationException("Já existe uma unidade com este CNPJ.");
        }

        return await _repository.CadastrarUnidade(unidade);
    }

    public async Task<bool> AtualizarAsync(UnidadeOrganizacional unidade)
    {
        if (!string.IsNullOrWhiteSpace(unidade.Cpnj))
        {
            var existe = await _repository.VerificaExisteUnidade(unidade.Cpnj, unidade.Id);
            if (existe) throw new InvalidOperationException("Este CNPJ já está a ser usado por outra unidade.");
        }

        return await _repository.AtualizarUnidade(unidade);
    }

    public async Task<bool> ExcluirAsync(int id) => await _repository.ExcluirUnidade(id);
}