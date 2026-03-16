namespace Zenite.InventarioVegetal.Server.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Validado { get; set; } = false;
        public PerfilUsuario Perfil { get; set; }
        public List<int> IdUnidadesOrganizacionais { get; set; } = new List<int>();
    }
}