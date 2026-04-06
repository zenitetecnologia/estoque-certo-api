using Estoque.Server.Models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class UsuarioController
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        app.MapPost("v1/usuarios/", async (Usuario usuario, UsuarioService service) =>
        {
            Guid usuarioId = await service.CadastrarUsuario(usuario);

            return Results.Created($"/v1/usuarios/{usuarioId}", "Usuário cadastrado com sucesso. Aguarde a aprovação do Administrador.");
        })
       .WithTags("usuarios")
       .WithSummary("Registra um novo usuário")
       .WithDescription("Registra um novo usuário com perfil normal e não validado.")
       .Produces(StatusCodes.Status201Created)
       .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
       .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapPut("v1/usuarios/{usuarioId:guid}", async (Guid usuarioId, UsuarioAtualizado usuario, UsuarioService service) =>
        {
            await service.AtualizarUsuario(usuario, usuarioId);

            return Results.Ok("Usuário atualizado com sucesso.");
        })
        .WithTags("usuarios")
        .WithSummary("Atualiza registro de usuário")
        .WithDescription("Atualiza os dados de um usuário existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapPatch("v1/usuarios/{usuarioId:guid}", async (Guid usuarioId, UsuarioService service) =>
        {
            await service.ValidarAcesso(usuarioId);

            return Results.Ok("Acesso do usuário validado com sucesso.");
        })
       .WithTags("usuarios")
       .WithSummary("Valida o acesso do usuário")
       .WithDescription("Valida o acesso do usuário no sistema.")
       .Produces(StatusCodes.Status200OK)
       .Produces<string>(StatusCodes.Status400BadRequest)
       .Produces<string>(StatusCodes.Status404NotFound)
       .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapDelete("v1/usuarios/{usuarioId:guid}", async (Guid usuarioId, UsuarioService service) =>
        {
            await service.ExcluirUsuario(usuarioId);

            return Results.Ok("Usuário excluído com sucesso.");
        })
        .WithTags("usuarios")
        .WithSummary("Exclui um usuário")
        .WithDescription("Exclui um usuário do sistema.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<string>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/usuarios", async (string? username, string? unidadeOrganizacionalId, UsuarioService service) =>
        {
            var result = await service.ObterUsuarios(username, unidadeOrganizacionalId);

            return Results.Ok(result);
        })
        .WithTags("usuarios")
        .WithSummary("Retorna lista de usuários")
        .WithDescription("Retorna uma lista de usuários de acordo com os parâmetros informados.")
        .Produces<List<UsuarioRecuperado>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/usuarios/{usuarioId:guid}", async (Guid usuarioId, UsuarioService service) =>
        {
            var result = await service.ObterUsuario(usuarioId);

            return Results.Ok(result);
        })
        .WithTags("usuarios")
        .WithSummary("Retorna um usuário por ID")
        .WithDescription("Retorna um usuário específico por ID.")
        .Produces<UsuarioRecuperado>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}