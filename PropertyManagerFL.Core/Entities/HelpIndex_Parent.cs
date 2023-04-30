using System;

namespace PropertyManagerFL.Core.Entities
{
    public class HelpIndex_Parent
    {
        public int Id { get; set; }
        public string? NomeProjeto { get; set; }
        public string? NomeExe { get; set; }
        public string? NomePdf { get; set; }
        public string? NomeWord { get; set; }
        public string?  Descricao { get; set; }
        public DateTime dCriacao { get; set; }

    }
}
