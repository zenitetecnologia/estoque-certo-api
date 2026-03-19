namespace Estoque.Server.Validations;

public class ValidationException : Exception
{
    public List<ValidationError> Errors { get; }

    public ValidationException(List<ValidationError> errors)
        : base("Ocorreram erros de validação.")
    {
        Errors = errors;
    }
}