namespace PropertyManagerFL.Core.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Morada { get; set; }
        public string? Localidade { get; set; }
        public string? Contacto { get; set; }
        public string? eMail { get; set; }
        public string? Notas { get; set; }
        public int ID_TipoContacto { get; set; }
    }
}
