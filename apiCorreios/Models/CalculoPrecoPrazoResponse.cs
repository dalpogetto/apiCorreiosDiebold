namespace apiCorreios.Models
{
    public class CalculoPrecoPrazoResponse
    {
        public List<Calculo> calculo { get; set; } = new();
    }

    public class Calculo
    {
        public string codProduto { get; set; }
        public string nomeProduto { get; set; }

        public decimal valor { get; set; }

        public string prazo { get; set; }

        public string mensagem { get; set; }
    }
}
