using Estoque.models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class UnidadeOrganizacionalController
{
    public static void MapUnidadeOrganizacionalEndpoints(this WebApplication app)
    {
        app.MapGet("v1/unidades-organizacionais", async (IUnidadeOrganizacionalService service) =>
        {
            try
            {
                var result = await service.ObterTodasAsync();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Lista as unidades organizacionais")
        .WithDescription("Retorna a lista de todas as unidades.")
        .Produces<List<UnidadeOrganizacional>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapGet("v1/unidades-organizacionais/{id:int}", async (int id, IUnidadeOrganizacionalService service) =>
        {
            try
            {
                var result = await service.ObterPorIdAsync(id);

                if (result == null)
                    return Results.NotFound(new { erro = "Unidade não encontrada." });

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Busca uma unidade por ID")
        .WithDescription("Obtém os detalhes de uma unidade específica.")
        .Produces<UnidadeOrganizacional>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapPost("v1/unidades-organizacionais", async (UnidadeOrganizacional unidade, IUnidadeOrganizacionalService service) =>
        {

            var id = await service.CadastrarAsync(unidade);

            unidade.Id = id;

            return Results.Created($"/v1/unidades-organizacionais/{id}", unidade);

        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Cadastra uma nova unidade")
        .WithDescription("Cria uma nova unidade organizacional.")
        .Produces<UnidadeOrganizacional>(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);


        app.MapPut("v1/unidades-organizacionais/{id:int}", async (int id, UnidadeOrganizacional unidade, IUnidadeOrganizacionalService service) =>
        {
            try
            {
                unidade.Id = id;
                var atualizado = await service.AtualizarAsync(unidade);

                if (!atualizado)
                    return Results.NotFound(new { erro = "Unidade não encontrada para atualizar." });

                return Results.Ok(new { mensagem = "Unidade atualizada com sucesso!" });
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ex.Errors);
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
        .WithTags("unidades-organizacionais")
        .WithSummary("Atualiza uma unidade")
        .WithDescription("Atualiza as informações de uma unidade existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);


        app.MapDelete("v1/unidades-organizacionais/{id:int}", async (int id, IUnidadeOrganizacionalService service) =>
        {
            try
            {
                var excluido = await service.ExcluirAsync(id);

                if (!excluido)
                    return Results.NotFound(new { erro = "Unidade não encontrada." });

                return Results.Ok(new { mensagem = "Unidade excluída com sucesso!" });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithTags("unidades-organizacionais")
        .WithSummary("Exclui uma unidade")
        .WithDescription("Remove uma unidade organizacional do sistema.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}