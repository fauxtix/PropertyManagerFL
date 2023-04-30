namespace PropertyManagerFL.Core.Entities
{
	public class Inquilino
	{
		public int Id { get; set; }
		public string? Nome { get; set; }
		public string? Morada { get; set; }
		public string? Naturalidade { get; set; }
		public DateTime DataNascimento { get; set; }
		public string? Contacto1 { get; set; }
		public string? Contacto2 { get; set; }
		public string? NIF { get; set; }
		public string? Identificacao { get; set; }
		public DateTime ValidadeCC { get; set; }
		public string? eMail { get; set; }
		public decimal IRSAnual { get; set; }
		public decimal Vencimento { get; set; }
		public bool Titular { get; set; }
		public string? Notas { get; set; }
		public bool Ativo { get; set; } = true;

        public decimal SaldoCorrente { get; set; }
        public decimal SaldoPrevisto { get; set; }

        // FK
        public int EstadoCivil { get; set; }
	}
}
