using Estoque.models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class UsuarioController
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        app.MapGet("v1/usuarios", async (IUsuarioService service) =>
        {
            var result = await service.ListarTodosAsync();

            return Results.Ok(result);

        })
        .WithTags("usuarios")
        .WithSummary("Lista os usuários")
        .WithDescription("Retorna a lista de todos os usuários.")
        .Produces<List<Usuario>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapGet("v1/usuarios/{id:int}", async (int id, IUsuarioService service) =>
        {
            var result = await service.ObterPorIdAsync(id);

            if (result == null)
                return Results.NotFound(new { erro = "Usuário não encontrado." });

            return Results.Ok(result);

        })
        .WithTags("usuarios")
        .WithSummary("Busca um usuário por ID")
        .WithDescription("Retorna um usuário específico pelo seu ID.")
        .Produces<Usuario>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapPost("v1/usuarios/registrar", async (Usuario usuario, IUsuarioService service) =>
        {
            int idNovoUsuario = await service.CriarUsuario(usuario);

            usuario.Id = idNovoUsuario;

            return Results.Created($"/v1/usuarios/{idNovoUsuario}", new
            {
                mensagem = "Usuário cadastrado com sucesso. Aguarde a aprovação do Administrador.",
                id = idNovoUsuario
            });

        })
        .WithTags("usuarios")
        .WithSummary("Registra um novo usuário")
        .WithDescription("Registra um novo usuário com perfil Normal e não validado.")
        .Produces(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapPut("v1/usuarios/{id:int}", async (int id, Usuario usuario, IUsuarioService service) =>
        {
            usuario.Id = id;
            var atualizado = await service.AtualizarUsuario(usuario);

            if (!atualizado)
                return Results.NotFound(new { erro = "Usuário não encontrado para atualizar." });

            return Results.Ok(new { mensagem = "Usuário atualizado com sucesso!" });

        })
        .WithTags("usuarios")
        .WithSummary("Atualiza um usuário")
        .WithDescription("Atualiza os dados de um usuário existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapDelete("v1/usuarios/{id:int}", async (int id, IUsuarioService service) =>
        {
            var excluido = await service.ExcluirAsync(id);

            if (!excluido)
                return Results.NotFound(new { erro = "Usuário não encontrado." });

            return Results.Ok(new { mensagem = "Usuário excluído com sucesso!" });

        })
        .WithTags("usuarios")
        .WithSummary("Exclui um usuário")
        .WithDescription("Exclui um usuário do sistema.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}