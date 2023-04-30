namespace PropertyManagerFL.Core.Entities
{
    public class Utilizador
    {
        public string? Login { get; set; }
        public string? Senha { get; set; }
        public List<Funcao> Funcoes { get; set; } = new();
    }
}
