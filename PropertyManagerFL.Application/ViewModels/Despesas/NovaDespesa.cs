namespace PropertyManagerFL.Application.ViewModels.Despesas
{
    public class NovaDespesa
    {
        public DateTime DataMovimento { get; set; } = DateTime.Now;
        public decimal Valor_Pago { get; set; } = 0;
        public string? NumeroDocumento { get; set; }
        public int ID_TipoDespesa { get; set; }
        public int ID_CategoriaDespesa { get; set; }
        public int ID_ModoPagamento { get; set; }
        public string? Notas { get; set; }
    }
}
