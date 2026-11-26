namespace PropertyManagerFL.Application.ViewModels.Arrendamentos
{
    /// <summary>
    /// Resultado da operação transacional de terminação de contrato.
    /// </summary>
    public class EndLeaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal OutstandingDebt { get; set; }
        public int TenantId { get; set; }
    }
}
