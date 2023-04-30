namespace PropertyManagerFL.Application.ViewModels.Recebimentos
{
	public class HistoricoAtualizacaoRenda
	{
		public int Id { get; set; }
		public DateTime DataAtualizacao { get; set; }
		public int ID_Atualizacao { get; set; }  // Coeficiente de atualizacao
		public int ID_Fracao { get; set; }
		public decimal Valor_Renda { get; set; }
	}
}
