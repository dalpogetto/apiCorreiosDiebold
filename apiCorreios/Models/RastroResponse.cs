namespace apiCorreios.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Endereco
    {
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string complemento { get; set; }
        public string numero { get; set; }
        public string bairro { get; set; }
    }

    public class Evento
    {
        public string codigo { get; set; }
        public string tipo { get; set; }
        public DateTime dtHrCriado { get; set; }
        public string descricao { get; set; }
        public string detalhe { get; set; }
        public Unidade unidade { get; set; }
        public UnidadeDestino unidadeDestino { get; set; }
        public string comentario { get; set; }
    }

    public class Objeto
    {
        public string codObjeto { get; set; }
        public TipoPostal tipoPostal { get; set; }
        public DateTime dtPrevista { get; set; }
        public string contrato { get; set; }
        public int largura { get; set; }
        public int comprimento { get; set; }
        public int altura { get; set; }
        public int diametro { get; set; }
        public double peso { get; set; }
        public string formato { get; set; }
        public string modalidade { get; set; }
        public int valorDeclarado { get; set; }
        public List<Evento> eventos { get; set; }
    }

    public class RastroResponse
    {
        public string versao { get; set; }
        public int quantidade { get; set; }
        public List<Objeto> objetos { get; set; }
        public string tipoResultado { get; set; }
    }

    public class TipoPostal
    {
        public string sigla { get; set; }
        public string descricao { get; set; }
        public string categoria { get; set; }
    }

    public class Unidade
    {
        public string codSro { get; set; }
        public string tipo { get; set; }
        public Endereco endereco { get; set; }
    }

    public class UnidadeDestino
    {
        public string tipo { get; set; }
        public Endereco endereco { get; set; }
    }


}
