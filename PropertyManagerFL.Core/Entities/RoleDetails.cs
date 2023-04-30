using System.ComponentModel.DataAnnotations;

namespace PropertyManagerFL.Core.Entities;
public class RoleDetails
{
    [Required]
    public Int32 Id { get; set; }

    [Required]
    [MaxLength(255)]
    public String? Descricao { get; set; }

}