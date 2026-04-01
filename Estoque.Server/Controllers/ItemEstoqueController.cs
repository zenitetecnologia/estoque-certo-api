using Estoque.Models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class ItemEstoqueController
{
    public static void MapItemEstoqueEndpoints(this WebApplication app)
    {
        app.MapPost("v1/itens-estoque", async (ItemEstoque item, ItemEstoqueService service) =>
        {
            Guid itemEstoqueId = await service.CadastrarItemEstoque(item);

            return Results.Created($"/v1/itens-estoque/{itemEstoqueId}", "Item cadastrado com sucesso.");
        })
        .WithTags("itens-estoque")
        .WithSummary("Cadastra um novo item de estoque")
        .WithDescription("Cadastra um novo item no estoque.")
        .Produces(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapPut("v1/itens-estoque/{itemEstoqueId:guid}", async (Guid itemEstoqueId, ItemEstoqueAtualizado item, ItemEstoqueService service) =>
        {
            await service.AtualizarItemEstoque(item, itemEstoqueId);

            return Results.Ok("Item de estoque atualizado com sucesso.");
        })
        .WithTags("itens-estoque")
        .WithSummary("Atualiza um item de estoque")
        .WithDescription("Atualiza as informações de um item de estoque existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapDelete("v1/itens-estoque/{itemEstoqueId:guid}", async (Guid itemEstoqueId, ItemEstoqueService service) =>
        {
            await service.ExcluirItemEstoque(itemEstoqueId);

            return Results.Ok("Item de estoque excluído com sucesso.");
        })
        .WithTags("itens-estoque")
        .WithSummary("Exclui um item de estoque")
        .WithDescription("Exclui um item do estoque permanentemente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/itens-estoque", async (ItemEstoqueService service) =>
        {
            var result = await service.ObterItens();

            return Results.Ok(result);
        })
        .WithTags("itens-estoque")
        .WithSummary("Lista os itens de estoque")
        .WithDescription("Lista todos os itens presentes no estoque.")
        .Produces<List<ItemEstoqueRecuperado>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/itens-estoque/{itemEstoqueId:guid}", async (Guid itemEstoqueId, ItemEstoqueService service) =>
        {
            var result = await service.ObterItem(itemEstoqueId);

            return Results.Ok(result);
        })
        .WithTags("itens-estoque")
        .WithSummary("Busca um item de estoque por ID")
        .WithDescription("Obtém os detalhes de um item pelo seu ID.")
        .Produces<ItemEstoqueRecuperado>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}