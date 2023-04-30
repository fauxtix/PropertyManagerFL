namespace PropertyManagerFL.Application.ViewModels.Imoveis
{
	public class NovoImovel
	{
		public string? Descricao { get; set; }
		public string? Morada { get; set; }
		public string? Numero { get; set; }
		public string? CodPst { get; set; }
		public string? CodPstEx { get; set; }
		public string? FreguesiaImovel { get; set; }
		public string? ConcelhoImovel { get; set; }
        public string? AnoConstrucao { get; set; }
        public DateTime DataUltimaInspecaoGas { get; set; }
        public bool Elevador { get; set; } = false;
        public int Conservacao { get; set; }
        public string? Notas { get; set; }
		public string? Foto { get; set; }
	}
}
