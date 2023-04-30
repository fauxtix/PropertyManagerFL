namespace PropertyManagerFL.Core.Entities
{
	public class Imovel
	{
		public int Id { get; set; }

		public string? Descricao { get; set; }
		public string? Morada { get; set; }
		public string? Numero { get; set; }
		public string? CodPst { get; set; }
		public string? CodPstEx { get; set; }
		public string? Freguesia { get; set; }
		public string? Concelho { get; set; }
		public string? AnoConstrucao { get; set; }
        public DateTime DataUltimaInspecaoGas { get; set; }
        public bool Elevador { get; set; } = false;
		public string? Notas { get; set; }
		public int Conservacao { get; set; }
		public string? Foto { get; set; }
		public List<Fracao>? Fracoes { get; set; } = new();
	}
}
