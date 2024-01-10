namespace PropertyManagerFL.Core.Entities;
public class Concelho
{
    public int Id { get; set; }
    public int CodConcelho { get; set; }
    public string? Descricao { get; set; }
    public int IdDistrito { get; set; }
    public float Coeficiente { get; set; }
}
