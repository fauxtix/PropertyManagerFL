namespace PropertyManagerFL.Core.Entities
{
    public class CC_Inquilino
    {
        public int Id { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorEmDivida { get; set; }
        public bool Renda { get; set; }
        public int ID_TipoRecebimento { get; set; }
        public string? Notas { get; set; }
        public int IdInquilino { get; set; }
    }
}
