using Estoque.Server.Models;
using Estoque.Server.Services;
using Estoque.Server.Validations;

namespace Estoque.Server.Controllers;

public static class UnidadeOrganizacionalController
{
    public static void MapUnidadeOrganizacionalEndpoints(this WebApplication app)
    {
        #region [ post ]
        app.MapPost("v1/unidades-organizacionais/", async (UnidadeOrganizacionalService service, UnidadeOrganizacional unidade) =>
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
        #endregion

        #region [ put ]
        app.MapPut("v1/unidades-organizacionais/{unidadeOrganizacionalId:Guid}", async (UnidadeOrganizacionalService service, Guid unidadeOrganizacionalId, UnidadeOrganizacional unidade) =>
        {
            await service.AtualizarUnidade(unidade, unidadeOrganizacionalId);

            return Results.Ok("Unidade atualizada com sucesso.");
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Atualiza uma unidade")
        .WithDescription("Atualiza as informações de uma unidade.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
        #endregion

        #region [ delete ]
        app.MapDelete("v1/unidades-organizacionais/{unidadeOrganizacionalId:Guid}", async (UnidadeOrganizacionalService service, Guid unidadeOrganizacionalId) =>
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
        #endregion

        #region [ get ]
        app.MapGet("v1/unidades-organizacionais", async (UnidadeOrganizacionalService service, int skip = 0, int top = 10, string? razaoSocial = null, string? Cnpj = null) =>
        {
            var result = await service.ObterUnidades(skip, top, razaoSocial, Cnpj);

            return Results.Ok(result);
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Obtém as unidades")
        .WithDescription("Obtém todas as unidades organizacionais.")
        .Produces<List<UnidadeOrganizacionalRecuperado>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);
        #endregion

        #region [ get by id ]
        app.MapGet("v1/unidades-organizacionais/{unidadeOrganizacionalId:Guid}", async (UnidadeOrganizacionalService service, Guid unidadeOrganizacionalId) =>
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
        #endregion
    }
}