namespace PropertyManagerFL.Core.Entities
{
    public class HelpIndex
    {
        public int Id { get; set; }
        public string? NomeForm { get; set; }
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public byte Pagina { get; set; }
        public int ID_Parent { get; set; }
    }
}
