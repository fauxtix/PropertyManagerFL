namespace PropertyManagerFL.Application.ViewModels.Inquilinos
{
    public class CC_InquilinoVM
    {
        public int Id { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorEmDivida { get; set; }
        public int IdInquilino { get; set; }
        public bool Renda { get; set; }
        public int ID_TipoRecebimento { get; set; }
        public int TransactionId { get; set; }
        public string? Notas { get; set; }
        public string? NomeInquilino { get; set; }
    }
}
