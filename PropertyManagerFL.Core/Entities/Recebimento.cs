namespace PropertyManagerFL.Core.Entities
{
	public class Recebimento
	{
		public int Id { get; set; }
		public DateTime DataMovimento { get; set; }
		public int EstadoPagamento { get; set; }
		public bool Renda { get; set; } = true;
		public decimal ValorPrevisto { get; set; }
		public decimal ValorRecebido { get; set; }
		public decimal ValorEmFalta { get; set; }
		public int ID_Propriedade { get; set; }
		public int ID_TipoRecebimento { get; set; }
		public int ID_Inquilino { get; set; }
		public string? Notas { get; set; }
		public bool GeradoPeloPrograma { get; set; } = false;
	}
}
