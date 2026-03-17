using Microsoft.AspNetCore.Mvc;
using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EspacosController : ControllerBase
{
    private readonly IEspacosService _service;

    public EspacosController(IEspacosService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os espaços de armazenamento.</summary>
    [HttpGet]
    public async Task<IActionResult> ListarTodos() => Ok(await _service.ObterTodosAsync());

    /// <summary>Obtém um espaço específico pelo seu ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var espaco = await _service.ObterPorIdAsync(id);
        if (espaco == null) return NotFound("Espaço não encontrado.");
        return Ok(espaco);
    }

    /// <summary>Cria um novo espaço de armazenamento.</summary>
    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] Espacos espaco)
    {
        try
        {
            var id = await _service.CadastrarAsync(espaco);
            return StatusCode(201, new { mensagem = "Espaço cadastrado com sucesso", id = id });
        }
        catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    /// <summary>Atualiza as informações de um espaço.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Espacos espaco)
    {
        try
        {
            espaco.Id = id;
            var atualizado = await _service.AtualizarAsync(espaco);
            if (!atualizado) return NotFound("Espaço não encontrado para atualizar.");
            return Ok(new { mensagem = "Espaço atualizado com sucesso!" });
        }
        catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    /// <summary>Remove um espaço do sistema.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var excluido = await _service.ExcluirAsync(id);
        if (!excluido) return NotFound("Espaço não encontrado.");
        return Ok(new { mensagem = "Espaço excluído com sucesso!" });
    }
}