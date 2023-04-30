using System.ComponentModel.DataAnnotations;

namespace PropertyManagerFL.Core.Entities;
public class User_Info
{
    [Required]
    public Int32 Id { get; set; }

    [Required]
    public Int32 RoleId { get; set; }

    [MaxLength(50)]
    public String? User_Name { get; set; }

    [MaxLength(50)]
    public String? Pwd { get; set; }

    [MaxLength(50)]
    public String? First_Name { get; set; }

    [MaxLength(255)]
    public String? EMail { get; set; }

    [MaxLength(255)]
    public String? Mobile { get; set; }

    public DateTime Last_Login_Date { get; set; }

    public DateTime Password_Change_Date { get; set; }

    public Int32 IsActive { get; set; }

}
