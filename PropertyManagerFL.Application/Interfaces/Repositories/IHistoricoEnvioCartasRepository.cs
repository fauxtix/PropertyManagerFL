using PropertyManagerFL.Application.Shared.Enums;
using PropertyManagerFL.Core.Entities;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public interface IHistoricoEnvioCartasRepository
{
    Task<IEnumerable<HistoricoEnvioCartas>> GetLettersSent();
    Task<HistoricoEnvioCartas> GetLetterSent(int Id);
    Task<int> InsertLetterSent(HistoricoEnvioCartas letterSent);
    Task<bool> UpdateLetterAnsweredDate(int Id, DateTime answerDate);
}