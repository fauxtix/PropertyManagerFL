namespace PropertyManagerFL.Application.ViewModels.Inquilinos
{
    public class RentAdjustmentsVM
    {
        public int Id { get; set; }
        public DateTime DataMovimento { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal ValorRenda { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorEmDivida { get; set; }
        public string? FracaoInquilino { get; set; } = "";
        public string? NomeInquilino { get; set; } = "";
        public string? Notas { get; set; } = "";
        public int TransactionId { get; set; }
    }
}
