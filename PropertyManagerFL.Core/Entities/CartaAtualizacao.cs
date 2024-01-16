namespace PropertyManagerFL.Core.Entities
{
    public class CartaAtualizacao : DadosOutorgante
    {
        public string? LocalEmissao { get; set; }
        public DateTime DataEmissao { get; set; }
        public string? AnoAtualizacao { get; set; }
        public string? Coeficiente { get; set; }
        public string? MatrizPredial { get; set; }
        public string? NomeInquilino { get; set; }
        public string? MoradaInquilino { get; set; }
        public string? MoradaFracao { get; set; }
        public decimal ValorRenda { get; set; }
        public decimal NovoValorRenda { get; set; }
        public string? NovoValorExtenso { get; set; }
        public string? DiaAPartirDe { get; set; }
        public string? MesAPartirDe { get; set; }
        public string? AnoAPartirDe { get; set; }
        public string? Lei { get; set; }
        public string? DataPublicacao { get; set; }
    }
}
