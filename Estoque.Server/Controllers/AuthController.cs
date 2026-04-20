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
        .Produces<AuthResponde>(StatusCodes.Status200OK)
        .Produces<List<ValidationError>>(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status401Unauthorized)
        .Produces<string>(StatusCodes.Status403Forbidden)
        .Produces<string>(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
        #endregion

        #region [ forgot ]
        app.MapPost("v1/auth/forgot", async (AuthService service, ForgotRequest request) =>
           {
               await service.EsquecerSenha(request);

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
    }
}