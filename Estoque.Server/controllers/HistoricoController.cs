using Estoque.models;
using Estoque.Services;

namespace Estoque.Controllers;

public static class HistoricoController
{
    public static void MapHistoricoEndpoints(this WebApplication app)
    {
        app.MapGet("v1/historicos/item/{idItem:int}", async (int idItem, HistoricoService service) =>
        {
            try
            {
                var lista = await service.ObterHistoricoPorItemAsync(idItem);

                return Results.Ok(lista);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("historicos")
        .WithSummary("Obtém o histórico de um item")
        .WithDescription("Obtém o histórico completo de movimentações de um item específico.")
        .Produces<List<Historico>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPost("v1/historicos", async (Historico historico, HistoricoService service) =>
        {
            try
            {
                var id = await service.RegistarMovimentacaoAsync(historico);
                historico.HistoricoId = id;

                return Results.Created($"/v1/historicos/{id}", historico);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("historicos")
        .WithSummary("Regista uma movimentação")
        .WithDescription("Regista uma entrada ou saída no histórico de movimentações.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}