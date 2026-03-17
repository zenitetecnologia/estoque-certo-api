using Microsoft.AspNetCore.Mvc;
using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UnidadeOrganizacionalController : ControllerBase
{
    private readonly IUnidadeOrganizacionalService _service;

    public UnidadeOrganizacionalController(IUnidadeOrganizacionalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListarTodas()
    {
        return Ok(await _service.ObterTodasAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var unidade = await _service.ObterPorIdAsync(id);
        if (unidade == null) return NotFound("Unidade não encontrada.");
        return Ok(unidade);
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] UnidadeOrganizacional unidade)
    {
        try
        {
            var id = await _service.CadastrarAsync(unidade);
            return StatusCode(201, new { mensagem = "Unidade criada com sucesso", id = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UnidadeOrganizacional unidade)
    {
        try
        {
            unidade.Id = id;
            var atualizado = await _service.AtualizarAsync(unidade);
            if (!atualizado) return NotFound("Unidade não encontrada para atualizar.");
            return Ok(new { mensagem = "Unidade atualizada com sucesso!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var excluido = await _service.ExcluirAsync(id);
        if (!excluido) return NotFound("Unidade não encontrada.");
        return Ok(new { mensagem = "Unidade excluída com sucesso!" });
    }
}