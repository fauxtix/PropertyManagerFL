using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IEstadoCivil
    {
        IEnumerable<EstadoCivil> GetAllEstadoCivil();
        EstadoCivil GetEstadoCivil(int Id);
        int GetId_EstadoCivil(string estadoCivil);
    }
}
