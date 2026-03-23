using Estoque.Server.Models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class UsuarioController
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        //listar todos
        app.MapGet("v1/usuarios", async (UsuarioService service) =>
        {
            var result = await service.ListarTodosAsync();

            return Results.Ok(result);

        })
        .WithTags("usuarios")
        .WithSummary("Lista os usuários")
        .WithDescription("Retorna a lista de todos os usuários.")
        .Produces<List<UsuarioRecuperado>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        //get por id
        app.MapGet("v1/usuarios/{id:int}", async (int id, UsuarioService service) =>
        {
            var result = await service.ObterPorIdAsync(id);

            if (result == null)
                return Results.NotFound(new { erro = "Usuário não encontrado." });

            return Results.Ok(result);

        })
        .WithTags("usuarios")
        .WithSummary("Busca um usuário por ID")
        .WithDescription("Retorna um usuário específico pelo seu ID.")
        .Produces<UsuarioRecuperado>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        //criar
        app.MapPost("v1/usuarios/", async (Usuario usuario, UsuarioService service) =>
        {
            int idNovoUsuario = await service.CriarUsuario(usuario);

            return Results.Created($"/v1/usuarios/{idNovoUsuario}", "Usuário cadastrado com sucesso. Aguarde a aprovação do Administrador.");
        })
        .WithTags("usuarios")
        .WithSummary("Registra um novo usuário")
        .WithDescription("Registra um novo usuário com perfil Normal e não validado.")
        .Produces(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        //atualizar
        app.MapPut("v1/usuarios/{id:int}", async (int id, Usuario usuario, UsuarioService service) =>
        {

            var atualizado = await service.AtualizarUsuario(usuario);

            return Results.Ok("Usuário atualizado com sucesso.");

        })
        .WithTags("usuarios")
        .WithSummary("Atualiza um usuário")
        .WithDescription("Atualiza os dados de um usuário existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<InvalidOperationException>(StatusCodes.Status400BadRequest)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        //delete
        app.MapDelete("v1/usuarios/{id:int}", async (int id, UsuarioService service) =>
        {
            var excluido = await service.ExcluirAsync(id);
   
            return Results.Ok(new { mensagem = "Usuário excluído com sucesso!" });

        })
        .WithTags("usuarios")
        .WithSummary("Exclui um usuário")
        .WithDescription("Exclui um usuário do sistema.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<InvalidOperationException>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        //vincular unidades
        app.MapPost("v1/usuarios/validar", async (UsuarioValido request, UsuarioService service) =>
        {
            await service.VincularComUnidades(request);

            return Results.Ok(new { mensagem = "Unidades vinculadas com sucesso!" });

        })
        .WithTags("usuarios")
        .WithSummary("Vincula unidades a um usuário")
        .WithDescription("Vincula usuário a uma ou mais unidades organizacionais.")
        .Produces(StatusCodes.Status200OK)
        .Produces<InvalidOperationException>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}