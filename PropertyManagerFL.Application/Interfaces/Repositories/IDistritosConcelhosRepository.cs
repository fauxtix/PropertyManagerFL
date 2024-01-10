using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public interface IDistritosConcelhosRepository
{
    Task<IEnumerable<Concelho>> GetConcelhosByDistrito(int id);
    Task<IEnumerable<Concelho>> GetConcelhos();
    Task<IEnumerable<LookupTableVM>> GetDistritos();

}
