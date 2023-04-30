namespace PropertyManagerFL.Application.ViewModels.Inquilinos
{
	/// <summary>
	/// Inquilino para insert
	/// </summary>
	public class NovoInquilino
	{
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

		// FK
		public int ID_EstadoCivil { get; set; }
	}
}
