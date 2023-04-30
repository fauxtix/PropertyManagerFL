using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.ViewModels.Messages
{
    public class ComposeMessageVM
    {
        public int MessageId { get; set; } 
        public string DestinationEmail { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string SubjectTitle { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public int MessageType { get; set; }
        public DateTime? MessageSentOn { get; set; }
        public DateTime? MessageReceivedOn { get; set; }
        public int TenantId { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();

    }
}
