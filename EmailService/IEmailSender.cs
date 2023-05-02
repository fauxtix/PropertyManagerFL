namespace EmailService
{
    public interface IEmailSender
    {
         Task SendEmailAsync(Message message);
        public bool IsValidEmail(string email);
    }
}
