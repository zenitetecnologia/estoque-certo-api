using Microsoft.AspNetCore.Mvc;
using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemEstoqueController : ControllerBase
{
    private readonly IItemEstoqueService _service;

    public ItemEstoqueController(IItemEstoqueService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os itens presentes no estoque.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarTodos()
    {
        var lista = await _service.ObterTodosAsync();
        return Ok(lista);
    }

    /// <summary>Obtém os detalhes de um item específico pelo seu ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var item = await _service.ObterPorIdAsync(id);
        if (item == null) return NotFound(new { erro = "Item de estoque não encontrado." });
        return Ok(item);
    }

    /// <summary>Regista um novo item no estoque.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar([FromBody] ItemEstoque item)
    {
        try
        {
            var id = await _service.CadastrarAsync(item);
            return StatusCode(201, new { mensagem = "Item cadastrado com sucesso!", id = id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno no servidor.", detalhe = ex.Message });
        }
    }

    /// <summary>Atualiza as informações de um item de estoque existente.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ItemEstoque item)
    {
        try
        {
            item.Id = id;
            var atualizado = await _service.AtualizarAsync(item);

            if (!atualizado) return NotFound(new { erro = "Item de estoque não encontrado para atualizar." });

            return Ok(new { mensagem = "Item de estoque atualizado com sucesso!" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno no servidor.", detalhe = ex.Message });
        }
    }

    /// <summary>Exclui um item do estoque permanentemente.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            var excluido = await _service.ExcluirAsync(id);
            if (!excluido) return NotFound(new { erro = "Item de estoque não encontrado." });

            return Ok(new { mensagem = "Item de estoque excluído com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno no servidor.", detalhe = ex.Message });
        }
    }
}