namespace PropertyManagerFL.Api.Models.Auth
{
    public class TokenModel
    {
        public string? Token { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
