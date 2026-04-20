namespace Estoque.Server.Models;

public class Auth
{
    public string Identificador { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class AuthResponde
{
    public string Token { get; set; } = string.Empty;
}

public class ForgotRequest
{
    public string Identificador { get; set; } = string.Empty;
}