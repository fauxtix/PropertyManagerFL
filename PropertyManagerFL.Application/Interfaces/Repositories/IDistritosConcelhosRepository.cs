using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public interface IDistritosConcelhosRepository
{
    Task<IEnumerable<Concelho>> GetConcelhosByDistrito(int id);
    Task<IEnumerable<DistritoConcelho>> GetConcelhos();
    Task<IEnumerable<LookupTableVM>> GetDistritos();
    Task UpdateCoeficienteIMI(int Id, decimal coeficienteIMI);
    Task<Concelho> GetConcelho_ById(int Id);
}
