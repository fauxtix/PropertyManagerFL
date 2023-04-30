using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagerFL.Core.Entities
{
    public class Funcao
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }

        public List<Utilizador>? Utilizadores { get; set; } = new();
    }
}
