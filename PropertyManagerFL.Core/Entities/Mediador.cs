using System;

namespace PropertyManagerFL.Core.Entities
{
    public class Mediador
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public string? Morada { get; set; }
        public string? Localidade { get; set; }
        public string? Contacto1 { get; set; }
        public string? Contacto2 { get; set; }
        public double Comissao { get; set; }
        public DateTime DataContacto { get; set; }
        public string? Notas { get; set; }
    }
}
