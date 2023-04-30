namespace PropertyManagerFL.Core.Entities
{
    public class AtualizacaoRenda
    {
        public int Id { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public float Coeficiente { get; set; }
        public string? Notas { get; set; }
    }
}
