namespace Estoque.Server.Validations;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
