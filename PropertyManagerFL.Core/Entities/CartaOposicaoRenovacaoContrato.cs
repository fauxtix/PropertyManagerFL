namespace PropertyManagerFL.Core.Entities
{
    public class CartaOposicaoRenovacaoContrato : DadosOutorgante
    {
        public string? LocalEmissao { get; set; }
        public DateTime DataEmissao { get; set; }
        public string? NomeInquilino { get; set; }
        public string? MoradaInquilino { get; set; }
        public string? MoradaFracao { get; set; } 
        public DateTime InicioContrato { get; set; }
        public DateTime FimContrato { get; set; }
    }
}
