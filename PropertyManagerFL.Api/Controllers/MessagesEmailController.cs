using EmailService;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.ViewModels.Email;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailMessagesController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailMessagesController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailMessage message)
        {
            var mailMessage = new Message(new string[] { message.Sender }, message.Subject, message.Body, null);
            await _emailSender.SendEmailAsync(mailMessage);
            return Ok(mailMessage);
        }
    }
}
