namespace PropertyManagerFL.Core.Entities
{
	public class DadosOutorgante
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public DateTime DataNascimento { get; set; }
        public string EstadoCivil { get; set; } = string.Empty;
		public string Identificação { get; set; } = string.Empty;
		public DateTime Validade_CC { get; set; }
		public string NIF { get; set; } = string.Empty;
		public string Naturalidade { get; set; } = string.Empty;
		public string Morada { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public int Freguesia { get; set; }
		public int Concelho { get; set; }
	}
}
