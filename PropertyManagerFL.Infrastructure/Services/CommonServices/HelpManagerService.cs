using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.Infrastructure.Services.CommonServices
{
    public class HelpManagerService : iHelpManagerService
    {
        readonly IHelpManagerRepository _repoHelp;
        public HelpManagerService(IHelpManagerRepository repoHelp)
        {
            _repoHelp = repoHelp;
        }

        public HelpViewModel GetHelpData(int IdProjeto, string NomeForm)
        {
            return _repoHelp.GetHelpData(IdProjeto, NomeForm);
        }

        public int GetIdProjeto(string sProjName)
        {
            return _repoHelp.GetIdProjeto(sProjName);
        }

        public bool HelpExists(int IdParent, string NomeForm)
        {
            return _repoHelp.HelpExists(IdParent, NomeForm);
        }
    }
}
