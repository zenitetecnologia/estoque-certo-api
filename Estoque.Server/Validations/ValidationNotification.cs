namespace Estoque.Server.Validations;

public class ValidationNotification
{
    private readonly List<ValidationError> _erros = new();

    public void ValidarTexto(string valor, string nomeDoCampo, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            _erros.Add(new ValidationError(nomeDoCampo, mensagem));
        }
    }

    public void AdicionarErro(ValidationError erro)
    {
        _erros.Add(erro);
    }

    public void DispararExcecao()
    {
        if (_erros.Any())
        {
            throw new ValidationException(_erros);
        }
    }
}
