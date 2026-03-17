using Microsoft.AspNetCore.Mvc;
using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HistoricoController : ControllerBase
{
    private readonly IHistoricoService _service;

    public HistoricoController(IHistoricoService service)
    {
        _service = service;
    }

    /// <summary>Obtém o histórico completo de movimentações de um item de estoque específico.</summary>
    [HttpGet("item/{idItem}")]
    public async Task<IActionResult> ObterHistoricoPorItem(int idItem)
    {
        var lista = await _service.ObterHistoricoPorItemAsync(idItem);
        return Ok(lista);
    }

    /// <summary>Regista uma entrada ou saída no histórico de movimentações (Auditoria).</summary>
    [HttpPost]
    public async Task<IActionResult> RegistarMovimentacao([FromBody] Historico historico)
    {
        try
        {
            var id = await _service.RegistarMovimentacaoAsync(historico);
            return StatusCode(201, new { mensagem = "Movimentação registada com sucesso", id = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno no servidor.", detalhe = ex.Message });
        }
    }
}