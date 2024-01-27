using PropertyManagerFL.Application.Shared.Enums;
using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface IHistoricoEnvioCartasService
{
    Task<IEnumerable<HistoricoEnvioCartasVM>> GetLettersSent();
    Task<bool> InsertLetterSent(HistoricoEnvioCartasVM letterSent, AppDefinitions.DocumentoEmitido letterType);
    Task<bool> UpdateLetterAnsweredDate(int Id, DateTime answerDate);

}
