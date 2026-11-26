namespace PropertyManagerFL.Application.ViewModels.Arrendamentos
{
    /// <summary>
    /// Dados necessários para terminar um contrato e arquivar inquilino/fiador.
    /// </summary>
    public class EndLeaseRequest
    {
        public DateTime LeaveDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
