using Estoque.Server.Validations;

namespace Estoque.Server.Services;

public class BaseService
{
    public List<ValidationError> Errors { get; set; } = new();

    protected void AddError(string field, string message)
    {
        Errors.Add(new ValidationError(field, message));
    }
}
