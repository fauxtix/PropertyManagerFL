namespace PropertyManagerFL.Application.ViewModels.Fiadores
{
	/// <summary>
	/// Fiador para insert
	/// </summary>
	public class NovoFiador
	{
		public int IdInquilino { get; set; }
		public string? Nome { get; set; }
		public string? Morada { get; set; }
		public string? Contacto1 { get; set; }
		public string? Contacto2 { get; set; }
		public string? NIF { get; set; }
		public string? Identificacao { get; set; }
		public DateTime ValidadeCC { get; set; }
		public string? eMail { get; set; }
		public decimal IRSAnual { get; set; }
		public decimal Vencimento { get; set; }
		public string? Notas { get; set; }

		// FK
		public int ID_EstadoCivil { get; set; }
	}
}
