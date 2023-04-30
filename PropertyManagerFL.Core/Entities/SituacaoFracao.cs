using System.ComponentModel.DataAnnotations;

namespace PropertyManagerFL.Core.Entities;
public class SituacaoFracao
{
    [Required]
    public int Id { get; set; }

    [MaxLength(30)]
    public string? Descricao { get; set; }

}
