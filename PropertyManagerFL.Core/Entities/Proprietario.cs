namespace PropertyManagerFL.Core.Entities
{
    public class Proprietario
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Morada { get; set; }
        public string? CodPostal { get; set; }
        public string? Naturalidade { get; set; }
        public int ID_EstadoCivil { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Contacto { get; set; }
        public string? NIF { get; set; }
        public string? Identificacao { get; set; }
        public DateTime ValidadeCC { get; set; }
        public string? eMail { get; set; }
        public string? Notas { get; set; }
    }
}
