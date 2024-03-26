namespace PropertyManagerFL.Application.ViewModels.Fracoes;
public class SeguroVM
{
    public int Id { get; set; }
    public int IdFracao { get; set; }
    public string? Fracao { get; set; }
    public string? Apolice { get; set; }
    public decimal Premio { get; set; }
    public string? Notas { get; set; }
}
