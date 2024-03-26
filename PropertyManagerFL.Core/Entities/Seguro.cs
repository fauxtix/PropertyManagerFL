namespace PropertyManagerFL.Core.Entities;
public class Seguro
{
    public int Id { get; set; }
    public int IdFracao { get; set; }
    public string? Apolice { get; set; }
    public decimal Premio { get; set; }
    public string? Notas { get; set; }
}
