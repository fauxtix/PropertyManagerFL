using System.ComponentModel.DataAnnotations;

namespace PropertyManagerFL.Api.Models.Auth
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "User Name é obrigatório!")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "IsAdmin é obrigatório!")]
        public bool IsAdmin { get; set; } = false;

        [EmailAddress]
        [Required(ErrorMessage = "Email é obrigatório!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password é obrigatório!")]
        public string? Password { get; set; }
    }
}
