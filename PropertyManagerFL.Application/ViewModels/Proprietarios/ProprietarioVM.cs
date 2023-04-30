using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagerFL.Application.ViewModels.Proprietarios
{
    public class ProprietarioVM
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Morada { get; set; }
        public string? Naturalidade { get; set; }
        public int ID_EstadoCivil { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Contacto { get; set; }
        public string? NIF { get; set; }
        public string? Identificacao { get; set; }
        public DateTime ValidadeCC { get; set; }
        public string? eMail { get; set; }
        public string? Notas { get; set; }
        public string? EstadoCivil { get; set; }
    }
}
