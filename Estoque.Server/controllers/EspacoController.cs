using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

public static class EspacosController
{
    public static void MapEspacosEndpoints(this WebApplication app)
    {
        app.MapGet("v1/espacos", async (IEspacosService service) =>
        {
            try
            {
                var result = await service.ObterTodosAsync();

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("espacos")
        .WithSummary("Lista os espaços")
        .WithDescription("Retorna a lista de espaços de armazenamento.")
        .Produces<List<Espacos>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapGet("v1/espacos/{id:int}", async (int id, IEspacosService service) =>
        {
            try
            {
                var result = await service.ObterPorIdAsync(id);

                if (result == null)
                    return Results.NotFound(new { erro = "Espaço não encontrado." });

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("espacos")
        .WithSummary("Busca um espaço por ID")
        .WithDescription("Retorna um espaço específico pelo seu ID.")
        .Produces<Espacos>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPost("v1/espacos", async (Espacos espaco, IEspacosService service) =>
        {
            try
            {
                var id = await service.CadastrarAsync(espaco);
                espaco.Id = id;

                return Results.Created($"/v1/espacos/{id}", espaco);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("espacos")
        .WithSummary("Cadastra um novo espaço")
        .WithDescription("Cadastra um novo espaço de armazenamento.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPut("v1/espacos/{id:int}", async (int id, Espacos espaco, IEspacosService service) =>
        {
            try
            {
                espaco.Id = id;
                var atualizado = await service.AtualizarAsync(espaco);

                if (!atualizado)
                    return Results.NotFound(new { erro = "Espaço não encontrado para atualizar." });

                return Results.Ok(new { mensagem = "Espaço atualizado com sucesso!" });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("espacos")
        .WithSummary("Atualiza um espaço")
        .WithDescription("Atualiza as informações de um espaço.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapDelete("v1/espacos/{id:int}", async (int id, IEspacosService service) =>
        {
            try
            {
                var excluido = await service.ExcluirAsync(id);

                if (!excluido)
                    return Results.NotFound(new { erro = "Espaço não encontrado." });

                return Results.Ok(new { mensagem = "Espaço excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("espacos")
        .WithSummary("Exclui um espaço")
        .WithDescription("Remove um espaço do sistema.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}