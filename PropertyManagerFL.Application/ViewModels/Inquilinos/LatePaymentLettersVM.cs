namespace PropertyManagerFL.Application.ViewModels.Inquilinos
{
    public class LatePaymentLettersVM
    {
        public string? Description { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public string? RefersTo { get; set; } = "";
        public string? NomeInquilino { get; set; } = "";
    }
}
