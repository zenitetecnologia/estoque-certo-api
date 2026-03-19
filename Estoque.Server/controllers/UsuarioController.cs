using Estoque.models;
using Estoque.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Controllers;

public static class UsuarioController
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        app.MapGet("v1/usuarios", async (IUsuarioService service) =>
        {
            try
            {
                var result = await service.ListarTodosAsync();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("usuarios")
        .WithSummary("Lista os usuários")
        .WithDescription("Retorna a lista de todos os usuários.")
        .Produces<List<Usuario>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapGet("v1/usuarios/{id:int}", async (int id, IUsuarioService service) =>
        {
            try
            {
                var result = await service.ObterPorIdAsync(id);

                if (result == null)
                    return Results.NotFound(new { erro = "Usuário não encontrado." });

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("usuarios")
        .WithSummary("Busca um usuário por ID")
        .WithDescription("Retorna um usuário específico pelo seu ID.")
        .Produces<Usuario>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPost("v1/usuarios", async (Usuario usuario, IUsuarioService service) =>
        {
            try
            {
                int idNovoUsuario = await service.RegistarUsuarioNormalAsync(usuario);

                return Results.Created($"/v1/usuarios/{idNovoUsuario}", new
                {
                    mensagem = "Usuário cadastrado com sucesso. Aguarde a aprovação do Administrador.",
                    id = idNovoUsuario
                });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("usuarios")
        .WithSummary("Registra um novo usuário")
        .WithDescription("Registra um novo usuário com perfil Normal e não validado.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPut("v1/usuarios/{id:int}", async (int id, Usuario usuario, IUsuarioService service) =>
        {
            try
            {
                usuario.Id = id;
                var atualizado = await service.AtualizarAsync(usuario);

                if (!atualizado)
                    return Results.NotFound(new { erro = "Usuário não encontrado para atualizar." });

                return Results.Ok(new { mensagem = "Usuário atualizado com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("usuarios")
        .WithSummary("Atualiza um usuário")
        .WithDescription("Atualiza os dados de um usuário existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPut("v1/usuarios", async (int id, [FromQuery] int idAdmin, IUsuarioService service) =>
        {
            try
            {
                await service.AprovarUsuarioAsync(idAdmin, id);
                return Results.Ok(new { mensagem = "Acesso de usuário aprovado com sucesso!" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Json(new { erro = ex.Message }, statusCode: 401);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("usuarios")
        .WithSummary("Aprova um usuário")
        .WithDescription("Aprova o acesso de um usuário (Apenas Admin).")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapDelete("v1/usuarios/{id:int}", async (int id, IUsuarioService service) =>
        {
            try
            {
                var excluido = await service.ExcluirAsync(id);

                if (!excluido)
                    return Results.NotFound(new { erro = "Usuário não encontrado." });

                return Results.Ok(new { mensagem = "Usuário excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("usuarios")
        .WithSummary("Exclui um usuário")
        .WithDescription("Exclui um usuário do sistema.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}