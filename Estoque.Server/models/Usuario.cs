namespace Estoque.models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public PerfilUsuario Perfil { get; set; }
        public List<UnidadeOrganizacional> UnidadesOrganizacionais { get; set; } = new List<UnidadeOrganizacional>();
    }
}