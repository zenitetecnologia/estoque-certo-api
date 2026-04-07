namespace Estoque.Server.Models;

public enum PerfilUsuario
{
    Admin = 1,
    Normal = 2
}

public enum TipoUnidadeMedida
{
    Quilos = 1,
    Gramas = 2,
    Miligramas = 2,
    Litros = 3,
    Mililitros = 4,
    Unidades = 5
}

public enum TipoMovimentacao
{
    Acrescimo = 1,
    Subtracao = 2
}