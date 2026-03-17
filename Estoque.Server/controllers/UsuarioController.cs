using Microsoft.AspNetCore.Mvc;
using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>Lista todos os utilizadores.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarUsuarios()
    {
        var lista = await _usuarioService.ListarTodosAsync();
        return Ok(lista);
    }

    /// <summary>Obtém um utilizador pelo seu ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);
        if (usuario == null) return NotFound(new { erro = "Utilizador não encontrado." });
        return Ok(usuario);
    }

    /// <summary>Registra um novo utilizador com perfil Normal e não validado.</summary>
    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
    {
        try
        {
            int idNovoUsuario = await _usuarioService.RegistarUsuarioNormalAsync(usuario);
            return StatusCode(201, new
            {
                mensagem = "Utilizador cadastrado com sucesso. Aguarde a aprovação do Administrador.",
                id = idNovoUsuario
            });
        }
        catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { erro = "Erro interno", detalhe = ex.Message }); }
    }

    /// <summary>Atualiza os dados de um utilizador existente.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Usuario usuario)
    {
        try
        {
            usuario.Id = id;
            var atualizado = await _usuarioService.AtualizarAsync(usuario);
            if (!atualizado) return NotFound(new { erro = "Utilizador não encontrado para atualizar." });
            return Ok(new { mensagem = "Utilizador atualizado com sucesso!" });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { erro = "Erro interno", detalhe = ex.Message }); }
    }

    /// <summary>Aprova o acesso de um utilizador (Apenas Admin).</summary>
    [HttpPut("{id}/aprovar")]
    public async Task<IActionResult> AprovarAcesso(int id, [FromQuery] int idAdmin)
    {
        try
        {
            await _usuarioService.AprovarUsuarioAsync(idAdmin, id);
            return Ok(new { mensagem = "Acesso de utilizador aprovado com sucesso!" });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { erro = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { erro = "Erro interno", detalhe = ex.Message }); }
    }

    /// <summary>Exclui um utilizador do sistema.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        var excluido = await _usuarioService.ExcluirAsync(id);
        if (!excluido) return NotFound(new { erro = "Utilizador não encontrado." });
        return Ok(new { mensagem = "Utilizador excluído com sucesso!" });
    }
}