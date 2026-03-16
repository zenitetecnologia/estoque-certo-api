namespace Zenite.InventarioVegetal.Server.models
{
    public class Espacos
    {
        public int Id { get; set; }
        public int IdUnidadeOrganizacional { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}
