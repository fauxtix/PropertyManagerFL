using System.ComponentModel.DataAnnotations;

namespace PropertyManagerFL.Core.Entities;
public class EstadoConservacao
{
    [Required]
    public Int32 Id { get; set; }

    [MaxLength(30)]
    public String? Descricao { get; set; }

}
