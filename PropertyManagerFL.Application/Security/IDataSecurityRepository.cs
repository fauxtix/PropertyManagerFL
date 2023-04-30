namespace PropertyManagerFL.Application.Security
{
    public interface IDataSecurityRepository
    {
        string Decrypt(string cryptedString);
        string Decrypt(string cryptedString, string field, int id);
        string Encrypt(string originalString);
    }
}