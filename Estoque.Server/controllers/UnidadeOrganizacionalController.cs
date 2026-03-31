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
            Guid unidadeOrganizacionalId = await service.CriarUnidade(unidade);

            return Results.Created($"/v1/unidades-organizacionais/{unidadeOrganizacionalId}", "Unidade Organizacional criada com sucesso.");
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Cria uma nova unidade")
        .WithDescription("Cria uma nova unidade organizacional no sistema.")
        .Produces(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapPut("v1/unidades-organizacionais/{unidadeOrganizacionalId:Guid}", async (Guid unidadeOrganizacionalId, UnidadeOrganizacional unidade, UnidadeOrganizacionalService service) =>
        {
            await service.AtualizarUnidade(unidade, unidadeOrganizacionalId);

            return Results.Ok("Unidade atualizada com sucesso.");
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Atualiza uma unidade")
        .WithDescription("Atualiza as informações de uma unidade existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapDelete("v1/unidades-organizacionais/{unidadeOrganizacionalId:Guid}", async (Guid unidadeOrganizacionalId, UnidadeOrganizacionalService service) =>
        {
            await service.ExcluirUnidade(unidadeOrganizacionalId);

            return Results.Ok("Unidade excluída com sucesso.");
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Exclui uma unidade")
        .WithDescription("Exclui uma unidade organizacional do sistema.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<string>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapGet("v1/unidades-organizacionais", async (UnidadeOrganizacionalService service) =>
        {
            var result = await service.ObterUnidades();

            return Results.Ok(result);
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Obtém as unidades")
        .WithDescription("Obtém todas as unidades organizacionais.")
        .Produces<List<UnidadeOrganizacionalRecuperado>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/unidades-organizacionais/{unidadeOrganizacionalId:Guid}", async (Guid unidadeOrganizacionalId, UnidadeOrganizacionalService service) =>
        {
            var result = await service.ObterUnidadePorId(unidadeOrganizacionalId);

            return Results.Ok(result);
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Obtém unidade por ID")
        .WithDescription("Obtém uma unidade específica.")
        .Produces<UnidadeOrganizacionalRecuperado>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}