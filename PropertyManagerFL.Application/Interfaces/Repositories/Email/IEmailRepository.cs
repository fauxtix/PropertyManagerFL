using PropertyManagerFL.Application.ViewModels.Email;

namespace PropertyManagerFL.Application.Interfaces.Repositories.Email
{
    public interface IEmailRepository
    {
        Task<string> SendEmailAsync(EmailMessage message);
        bool IsValidEmail(string EmailName);
    }
}
