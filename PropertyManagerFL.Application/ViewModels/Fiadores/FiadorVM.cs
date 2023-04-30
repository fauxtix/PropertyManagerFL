namespace PropertyManagerFL.Application.ViewModels.Fiadores
{
    public class FiadorVM
    {
        public int Id { get; set; }
        public int IdInquilino { get; set; }
        public bool Ativo { get; set; } = true;
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
        public int Contrato { get; set; }
        public int ID_EstadoCivil { get; set; }
        public string? EstadoCivil { get; set; }


    }
}
