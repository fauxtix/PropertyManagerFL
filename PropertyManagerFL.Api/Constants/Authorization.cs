namespace PropertyManagerFL.Api.Constants
{
    public class Authorization
    {
        public enum Roles
        {
            Administrator, Moderator, User
        }

        public const string default_username = "fausto";
        public const string default_email = "fauxtix.luix@propertymanager.pt";
        public const string default_password = "Admin123£.";
        public const Roles default_role = Roles.Administrator;
    }
}
