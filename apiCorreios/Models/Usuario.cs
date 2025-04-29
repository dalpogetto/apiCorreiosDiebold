namespace apiCorreios.Models
{
    public class Usuario
    {
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";
    }
}
