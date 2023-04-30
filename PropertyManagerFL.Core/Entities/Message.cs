namespace PropertyManagerFL.Core.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string DestinationEmail { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string SubjectTitle { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public int MessageType { get; set; }
        public DateTime? MessageSentOn { get; set; }
        public DateTime? MessageReceivedOn { get; set; }
        public int TenantId{ get; set; }
    }
}
