namespace PropertyManagerFL.Core.Entities
{
    public class CartaRendasAtraso : DadosOutorgante
    {
        public string LocalEmissao { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public string NomeInquilino { get; set; } = string.Empty;
        public string MoradaInquilino { get; set; } = string.Empty;
        public decimal MontanteRendasAtraso { get; set; }
        public string ValorExtenso { get; set; } = string.Empty;
        public string RendasEmAtraso { get; set; } = string.Empty;
        public string PrazoEmDias { get; set; } = string.Empty;

    }
}
