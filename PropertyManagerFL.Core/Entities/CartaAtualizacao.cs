namespace PropertyManagerFL.Core.Entities
{
    public class CartaAtualizacao : DadosOutorgante
    {
        public string LocalEmissao { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public string AnoAtualizacao { get; set; } = string.Empty;
        public string Coeficiente { get; set; } = string.Empty;
        public string MatrizPredial { get; set; } = string.Empty;
        public string NomeInquilino { get; set; } = string.Empty;
        public string MoradaInquilino { get; set; } = string.Empty;
        public string MoradaFracao { get; set; } = string.Empty;
        public decimal ValorRenda { get; set; }
        public decimal NovoValorRenda { get; set; }
        public string NovoValorExtenso { get; set; } = string.Empty;
        public string DiaAPartirDe { get; set; } = string.Empty;
        public string MesAPartirDe { get; set; } = string.Empty;
        public string AnoAPartirDe { get; set; } = string.Empty;
        public string Lei { get; set; } = string.Empty;
        public string DataPublicacao { get; set; } = string.Empty;
    }
}
