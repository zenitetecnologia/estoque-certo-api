using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

public static class ItemEstoqueController
{
    public static void MapItemEstoqueEndpoints(this WebApplication app)
    {
        app.MapGet("v1/itens-estoque", async (ItemEstoqueService service) =>
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
        .WithTags("itens-estoque")
        .WithSummary("Lista os itens de estoque")
        .WithDescription("Lista todos os itens presentes no estoque.")
        .Produces<List<ItemEstoque>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapGet("v1/itens-estoque/{id:int}", async (int id, ItemEstoqueService service) =>
        {
            try
            {
                var result = await service.ObterPorIdAsync(id);

                if (result == null)
                    return Results.NotFound(new { erro = "Item de estoque não encontrado." });

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("itens-estoque")
        .WithSummary("Busca um item de estoque por ID")
        .WithDescription("Obtém os detalhes de um item específico pelo seu ID.")
        .Produces<ItemEstoque>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPost("v1/itens-estoque", async (ItemEstoque item, ItemEstoqueService service) =>
        {
            try
            {
                var id = await service.CadastrarAsync(item);
                item.ItemEstoqueId = id;

                return Results.Created($"/v1/itens-estoque/{id}", item);
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
        .WithTags("itens-estoque")
        .WithSummary("Cadastra um novo item de estoque")
        .WithDescription("Regista um novo item no estoque.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPut("v1/itens-estoque/{id:int}", async (int id, ItemEstoque item, ItemEstoqueService service) =>
        {
            try
            {
                item.ItemEstoqueId = id;
                var atualizado = await service.AtualizarAsync(item);

                if (!atualizado)
                    return Results.NotFound(new { erro = "Item de estoque não encontrado para atualizar." });

                return Results.Ok(new { mensagem = "Item de estoque atualizado com sucesso!" });
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
        .WithTags("itens-estoque")
        .WithSummary("Atualiza um item de estoque")
        .WithDescription("Atualiza as informações de um item de estoque existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapDelete("v1/itens-estoque/{id:int}", async (int id, ItemEstoqueService service) =>
        {
            try
            {
                var excluido = await service.ExcluirAsync(id);

                if (!excluido)
                    return Results.NotFound(new { erro = "Item de estoque não encontrado." });

                return Results.Ok(new { mensagem = "Item de estoque excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("itens-estoque")
        .WithSummary("Exclui um item de estoque")
        .WithDescription("Exclui um item do estoque permanentemente.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}