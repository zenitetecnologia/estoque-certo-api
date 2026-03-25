using Estoque.models;
using Estoque.Repositories;

namespace Estoque.Services;


public class EspacoService
{
    private readonly EspacoRepository _repository;

    public EspacoService(EspacoRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> CriarEspaco(Espacos espaco)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(espaco.Nome))
                throw new ArgumentException("O nome do espaço é obrigatório.");

            if (espaco.UnidadeOrganizacionalId <= 0)
                throw new ArgumentException("A Unidade Organizacional é obrigatória.");

            return await _repository.CadastrarEspaco(espaco);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar espaço: {ex.Message}");
        }
    }






    public async Task<List<Espacos>> ObterTodosAsync()
    {
        try
        {
            return await _repository.ObterEspacos();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar todos os espaços: {ex.Message}");
        }
    }

    public async Task<Espacos?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _repository.ObterEspacoPorId(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar espaço por ID: {ex.Message}");
        }
    }



    public async Task<bool> AtualizarAsync(EspacosRecuperado espaco)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(espaco.Nome))
                throw new ArgumentException("O nome do espaço é obrigatório.");

            return await _repository.AtualizarEspaco(espaco);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar espaço: {ex.Message}");
        }
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        try
        {
            return await _repository.ExcluirEspaco(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao excluir espaço: {ex.Message}");
        }
    }

    public async Task ValidarEspaco(Espacos espacos, int espacoId)
    {

    }
}