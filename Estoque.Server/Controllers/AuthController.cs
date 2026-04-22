using Estoque.Server.Models;
using Estoque.Server.Services;
using Estoque.Server.Validations;

namespace Estoque.Server.Controllers;

public static class AuthController
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        #region [ post ]
        app.MapPost("v1/auth/login", async (AuthService service, Auth auth) =>
        {
            var response = await service.Login(auth);

            return Results.Ok(response);
        })
        .WithTags("auth")
        .WithSummary("Autentica um usuário")
        .WithDescription("Realiza o login e gera um Token JWT informando o username ou telefone e a senha.")
        .Produces<AuthToken>(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status401Unauthorized)
        .Produces<string>(StatusCodes.Status403Forbidden)
        .Produces<string>(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
        #endregion

        #region [ forgot ]
        app.MapPost("v1/auth/forgot", async (AuthService service, Auth auth) =>
        {
            await service.EsquecerSenha(auth);

            return Results.Ok(new { mensagem = "Se as informações estiverem corretas, você receberá um código de recuperação." });
        })
        .WithTags("auth")
        .WithSummary("Solicita recuperação de acesso")
        .WithDescription("Inicia o processo de recuperação de conta informando o username ou o telefone.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
        #endregion

        #region [ verify ]
        app.MapPost("v1/auth/verify", async (AuthService service, Auth auth) =>
        {
            await service.VerificarCodigo(auth);

            return Results.Ok(new { mensagem = "Código validado com sucesso." });
        })
        .WithTags("auth")
        .WithSummary("Verifica o código SMS")
        .WithDescription("Valida o código recebido para recuperação ou ativação de conta.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
        #endregion

        #region [ reset ]
        app.MapPost("v1/auth/reset", async (AuthService service, AuthReset auth) =>
        {
            await service.RedefinirSenha(auth);

            return Results.Ok(new { mensagem = "Senha redefinida com sucesso." });
        })
        .WithTags("auth")
        .WithSummary("Redefine a senha do usuário")
        .WithDescription("Altera a senha de acesso informando o username ou telefone e a nova senha.")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
        #endregion
    }
}