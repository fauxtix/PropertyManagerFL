namespace PropertyManagerFL.Application.ViewModels.Email
{
    public class EmailSettings
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string Title { get; set; } = "";
    }
}
