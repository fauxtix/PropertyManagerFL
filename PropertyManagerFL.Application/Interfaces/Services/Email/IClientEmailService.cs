using EmailService;

namespace PropertyManagerFL.Application.Interfaces.Services.Email
{
    public interface IClientEmailService
    {
        Task SendEmailAsync(Message message);
        public Task<bool> IsValidEmail(string email);
    }
}
