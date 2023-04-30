namespace PropertyManagerFL.Application.Interfaces.Services.Security
{
    public interface IDataSecurityService
    {
        string Decrypt(string cryptedString);
        string Decrypt(string cryptedString, string sCampo, int id);
        string Encrypt(string originalString);
    }
}