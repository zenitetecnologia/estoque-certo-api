namespace Estoque.models
{
    public enum PerfilUsuario
    {
        Admin = 1,
        Normal = 2
    }

    public enum TipoUnidadeMedida
    {
        Quilos,
        Gramas,
        Miligramas,
        Litros,
        Mililitros,
        Unidades
    }

    public enum TipoMovimentacao
    {
        Acrescimo,
        Subtracao
    }
}