using PropertyManagerFL.Application.Interfaces.Repositories.Email;
using PropertyManagerFL.Application.ViewModels.Email;
using PropertyManagerFL.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailSettings _mailConfig;
        private static string _mailResponse;

        public EmailRepository(EmailSettings mailConfig)
        {
            _mailConfig = mailConfig;
        }


        public async Task<string> SendEmailAsync(EmailMessage mensagem)
        {
            _mailResponse = string.Empty;

            using (SmtpClient smtpClient = new SmtpClient(_mailConfig.Host, _mailConfig.Port))
            {
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(_mailConfig.Username, _mailConfig.Password);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.SendCompleted += new SendCompletedEventHandler((sender, e) =>
                {
                    _mailResponse = e.Error != null || e.Cancelled != false ? "failure" : "success";
                });

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(_mailConfig.Username, _mailConfig.DisplayName),
                    Subject = mensagem.Body,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = mensagem.Body,
                    Priority = MailPriority.High
                };
                message.To.Add(new MailAddress(mensagem.Recipient));

                await smtpClient.SendMailAsync(message);
            }

            return _mailResponse;
        }

        public bool IsValidEmail(string EmailName)
        {
            return new EmailAddressAttribute().IsValid(EmailName);
        }

        private string GetEmailContent(string Title, EventModel Data)
        {
            string HTMLBody = string.Empty;

            using (FileStream fs = File.Open(Directory.GetCurrentDirectory() + "/Email_Template.html", FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    HTMLBody = sr.ReadToEnd();
                }
            }

            HTMLBody = HTMLBody.Replace("###EMAILTITLE###", Title);
            HTMLBody = HTMLBody.Replace("###EVENTSUBJECT###", Data.Subject ?? "(No Title)");
            HTMLBody = HTMLBody.Replace("###EVENTSTART###", Data.StartTime.ToString());
            HTMLBody = HTMLBody.Replace("###EVENTEND###", Data.EndTime.ToString());
            HTMLBody = HTMLBody.Replace("###EVENTLOCATION###", Data.Location ?? "NA");
            HTMLBody = HTMLBody.Replace("###EVENTDETAILS###", Data.Description ?? "NA");
            HTMLBody = HTMLBody.Replace("###CURRENTYEAR###", DateTime.Now.Year.ToString());

            return HTMLBody;
        }
    }
}