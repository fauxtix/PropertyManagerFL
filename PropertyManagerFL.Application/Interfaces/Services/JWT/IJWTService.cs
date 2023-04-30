using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.JWT;

public interface IJwtService
{
    string GerarToken(Utilizador utilizador);
}