namespace PropertyManagerFL.Application.ViewModels.Inquilinos
{
    public class HistoricoAtualizacaoRendasVM
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public DateTime DateProcessed { get; set; }
        public decimal PriorValue { get; set; }
        public decimal UpdatedValue { get; set; }
        public string? DescricaoFracao { get; set; }
        public string? NomeInquilino { get; set; }

    }
}
