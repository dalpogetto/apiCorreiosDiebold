namespace apiCorreios.Models
{
    public class CalculoPrecoPrazoRequest
    {
        public string cepOrigem { get;set; }
        public string cepDestino { get; set; }

        public List<Item> itens { get; set; }
    }
}
