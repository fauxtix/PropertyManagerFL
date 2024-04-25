namespace PropertyManagerFL.Core.Entities;
public class Condominio
{
    public int Id { get; set; }
    public int IdFracao { get; set; }
    public decimal Quota { get; set; }
    public string Ano { get; set; } = string.Empty;
    public string? Gestor { get; set; } 
    public string? ContactoGestor { get; set; }
    public decimal ValorAvenca { get; set; }
}
