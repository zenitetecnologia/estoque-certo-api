using Estoque.models;
using Estoque.Server.Validations;
using Estoque.Services;

namespace Estoque.Controllers;

public static class EspacoController
{
    public static void MapEspacosEndpoints(this WebApplication app)
    {
        app.MapPost("v1/espacos", async (Espaco espaco, EspacoService service) =>
        {
            Guid espacoId = await service.CadastrarEspaco(espaco);

            return Results.Created($"/v1/espacos/{espacoId}", "Espaço cadastrado com sucesso.");
        })
        .WithTags("espacos")
        .WithSummary("Cadastra um novo espaço")
        .WithDescription("Cadastra um novo espaço de armazenamento.")
        .Produces(StatusCodes.Status201Created)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapPut("v1/espacos/{espacoId:guid}", async (Guid espacoId, EspacoAtualizado espaco, EspacoService service) =>
        {
            await service.AtualizarEspaco(espaco, espacoId);

            return Results.Ok("Espaço atualizado com sucesso.");
        })
        .WithTags("espacos")
        .WithSummary("Atualiza um espaço")
        .WithDescription("Atualiza as informações de um espaço de armazenamento existente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapDelete("v1/espacos/{espacoId:guid}", async (Guid espacoId, EspacoService service) =>
        {
            await service.ExcluirEspaco(espacoId);

            return Results.Ok("Espaço excluído com sucesso.");
        })
        .WithTags("espacos")
        .WithSummary("Exclui um espaço")
        .WithDescription("Remove um espaço do sistema permanentemente.")
        .Produces(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/espacos", async (EspacoService service) =>
        {
            var result = await service.ObterEspacos();

            return Results.Ok(result);
        })
        .WithTags("espacos")
        .WithSummary("Lista os espaços")
        .WithDescription("Retorna a lista completa de espaços de armazenamento.")
        .Produces<List<EspacoRecuperado>>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("v1/espacos/{espacoId:guid}", async (Guid espacoId, EspacoService service) =>
        {
            var result = await service.ObterEspaco(espacoId);

            return Results.Ok(result);
        })
        .WithTags("espacos")
        .WithSummary("Busca um espaço por ID")
        .WithDescription("Retorna um espaço específico pelo seu ID.")
        .Produces<EspacoRecuperado>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}