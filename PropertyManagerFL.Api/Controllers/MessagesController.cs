using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Messages;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessagesController> _logger;
        public MessagesController(IMessageRepository messageRepository, ILogger<MessagesController> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var location = GetControllerActionNames();
            try
            {

                var userMessages = await _messageRepository.GetAllMessages();
                var composeMessageVM = new ComposeMessageVM()
                {
                    Messages = userMessages
                };

                return Ok(composeMessageVM.Messages);

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("{Id:int}")]
        public async Task<IActionResult> Get(int Id)
        {
            var location = GetControllerActionNames();

            try
            {
                var userMessage = await _messageRepository.GetMessageById(Id);
                var composeMessageVM = new ComposeMessageVM()
                {
                    DestinationEmail = userMessage.DestinationEmail,
                    SenderEmail = userMessage.SenderEmail,
                    SubjectTitle = userMessage.SubjectTitle,
                    MessageContent = userMessage.MessageContent,
                    MessageType = userMessage.MessageType,
                    MessageReceivedOn = userMessage.MessageReceivedOn,
                    MessageSentOn = userMessage.MessageSentOn
                };

                return Ok(composeMessageVM);

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }
        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var location = GetControllerActionNames();

            try
            {
                await _messageRepository.Delete(Id);

                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ComposeMessageVM messageVM)
        {
            var location = GetControllerActionNames();

            try
            {
                var message = new Message()
                {
                    DestinationEmail = messageVM.DestinationEmail,
                    SubjectTitle = messageVM.SubjectTitle,
                    SenderEmail = messageVM.SenderEmail,
                    MessageContent = messageVM.MessageContent,
                    MessageType = messageVM.MessageType,
                    MessageReceivedOn = messageVM.MessageReceivedOn,
                    MessageSentOn = messageVM.MessageSentOn,
                    TenantId = messageVM.TenantId,
                    ReferenceId = messageVM.ReferenceId,
                };

                var insertedId = await _messageRepository.Add(message);
                var createdMessage = await _messageRepository.GetMessageById(insertedId);
                var result = CreatedAtAction(nameof(Get), new { Id = insertedId }, createdMessage);
                return result;

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] ComposeMessageVM messageVM)
        {
            var location = GetControllerActionNames();
            try
            {
                if (messageVM == null || id != messageVM.MessageId)
                    return BadRequest();
                if (_messageRepository.GetMessageById(id) == null)
                {
                    return NotFound();
                }
                var messageToUpdate = new Message()
                {
                    MessageId = messageVM.MessageId,
                    DestinationEmail = messageVM.DestinationEmail,
                    SubjectTitle = messageVM.SubjectTitle,
                    SenderEmail = messageVM.SenderEmail,
                    MessageContent = messageVM.MessageContent,
                    MessageType = messageVM.MessageType,
                    MessageReceivedOn = messageVM.MessageReceivedOn,
                    MessageSentOn = messageVM.MessageSentOn,
                    TenantId = messageVM.TenantId,
                    ReferenceId = messageVM.ReferenceId,
                };

                await _messageRepository.Save(messageToUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }


        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
        }

    }
}
