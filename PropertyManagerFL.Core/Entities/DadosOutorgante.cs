namespace PropertyManagerFL.Core.Entities
{
	public class DadosOutorgante
	{
		public int Id { get; set; }
		public string? Nome { get; set; }
		public DateTime DataNascimento { get; set; }
		public string? EstadoCivil { get; set; }
		public string? Identificação { get; set; }
		public DateTime Validade_CC { get; set; }
		public string? NIF { get; set; }
		public string? Naturalidade { get; set; }
		public string? Morada { get; set; }
        public string? CodigoPostal { get; set; }
        public int Freguesia { get; set; }
		public int Concelho { get; set; }
	}
}
