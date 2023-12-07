namespace PropertyManagerFL.Core.Entities
{
    public class StatAtivos
    {
        public int Ativos { get; set; }
        public int NaoAtivos { get; set; }
    }

    public class EntityCount
    {
        public bool Ativo { get; set; }
        public int Contagem { get; set; }
        public double Percentagem { get; set; }
        public string? Descricao { get; set; }
    }

    public class ExpensesSummaryData
    {
        public string? Descricao { get; set; }
        public int NumeroMovimentos { get; set; }
        public double TotalDespesas { get; set; }
    }

    public class PaymentsSummaryData
    {
        public string? Descricao { get; set; }
        public int NumeroMovimentos { get; set; }
        public double TotalPagamentos { get; set; }
    }
}
