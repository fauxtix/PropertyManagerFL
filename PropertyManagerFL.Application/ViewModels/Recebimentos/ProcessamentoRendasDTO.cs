namespace PropertyManagerFL.Application.ViewModels.Recebimentos
{
    public class ProcessamentoRendasDTO
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public DateTime DataProcessamento { get; set; }
        public DateTime DataReferencia { get; set; }
        public decimal TotalRecebido { get; set; }
    }
}
