using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface IDistritosConcelhosService
{
    Task<IEnumerable<Concelho>> GetConcelhosByDistrito(int id);
    Task<IEnumerable<Concelho>> GetConcelhos();
    Task<IEnumerable<LookupTableVM>> GetDistritos();

}
