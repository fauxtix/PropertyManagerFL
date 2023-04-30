namespace PropertyManagerFL.Application.ViewModels.Logins
{
    public class LoginLogVM
    {
        public string UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public string SessionId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}
