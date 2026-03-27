using Estoque.models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class UnidadeOrganizacionalController
{
    public static void MapUnidadeOrganizacionalEndpoints(this WebApplication app)
    {

        app.MapPost("v1/unidades-organizacionais/", async (UnidadeOrganizacional unidade, UnidadeOrganizacionalService service) =>
        {
            int idNovaUnidade = await service.CriarUnidade(unidade);

            return Results.Created($"/v1/unidades-organizacionais/{idNovaUnidade}", "Unidade Organizacional cadastrada com sucesso.");
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Registra uma nova unidade organizacional")
        .WithDescription("Registra uma nova unidade organizacional no sistema.")
        .Produces(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapPut("v1/unidades-organizacionais/{id}", async (int unidadeId, UnidadeOrganizacional unidade, UnidadeOrganizacionalService service) =>
        {
            await service.AtualizarUnidade(unidade, unidadeId);

            return Results.Ok("Unidade atualizada com sucesso");

        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Atualiza uma unidade")
        .WithDescription("Atualiza as informações de uma unidade existente.")
        .Produces<UnidadeOrganizacional>(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);











        app.MapGet("v1/unidades-organizacionais", async (UnidadeOrganizacionalService service) =>
        {
            var result = await service.ObterUnidades();

            return Results.Ok(result);

        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Lista as unidades organizacionais")
        .WithDescription("Retorna a lista de todas as unidades.")
        .Produces<List<UnidadeOrganizacional>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/unidades-organizacionais/{id:int}", async (int id, UnidadeOrganizacionalService service) =>
        {
            var result = await service.ObterPorIdAsync(id);

            if (result == null)
                return Results.NotFound(new { erro = "Unidade não encontrada." });

            return Results.Ok(result);

        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Busca uma unidade por ID")
        .WithDescription("Obtém os detalhes de uma unidade específica.")
        .Produces<UnidadeOrganizacional>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapDelete("v1/unidades-organizacionais/{id:int}", async (int id, UnidadeOrganizacionalService service) =>
        {
            var excluido = await service.ExcluirUnidade(id);

            if (!excluido)
                return Results.NotFound(new { erro = "Unidade não encontrada." });

            return Results.NoContent();

        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Exclui uma unidade")
        .WithDescription("Remove uma unidade organizacional do sistema.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}