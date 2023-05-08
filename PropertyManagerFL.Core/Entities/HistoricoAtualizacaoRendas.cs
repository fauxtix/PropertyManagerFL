namespace PropertyManagerFL.Core.Entities;

public class HistoricoAtualizacaoRendas
{
    public int Id { get; set; }
    public int Id_Inquilino { get; set; }
    public DateTime DataMovimento { get; set; }
    public decimal ValorAnterior { get; set; }
    public decimal ValorAtualizado { get; set; }

}
