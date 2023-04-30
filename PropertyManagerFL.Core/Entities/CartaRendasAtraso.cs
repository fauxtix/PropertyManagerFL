namespace PropertyManagerFL.Core.Entities
{
    public class CartaRendasAtraso : DadosOutorgante
    {
        public string? LocalEmissao { get; set; }
        public DateTime DataEmissao { get; set; }
        public string? NomeInquilino { get; set; }
        public string? MoradaInquilino { get; set; }
        public decimal MontanteRendasAtraso { get; set; }
        public string? RendasEmAtraso { get; set; }
        public string? PrazoEmDias { get; set; }

    }
}
