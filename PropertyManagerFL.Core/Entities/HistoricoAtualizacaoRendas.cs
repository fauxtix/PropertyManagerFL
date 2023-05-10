namespace PropertyManagerFL.Core.Entities;

public class HistoricoAtualizacaoRendas
{
    public int Id { get; set; }
    public int UnitId { get; set; }
    public DateTime DateProcessed { get; set; }
    public decimal PriorValue { get; set; }
    public decimal UpdatedValue { get; set; }

}
