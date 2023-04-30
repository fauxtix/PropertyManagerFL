using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperHelpManager : iHelpManagerService
    {
        public HelpViewModel GetHelpData(int IdProjeto, string NomeForm)
        {
            throw new NotImplementedException();
        }

        public int GetIdProjeto(string sProjName)
        {
            throw new NotImplementedException();
        }

        public bool HelpExists(int IdParent, string NomeForm)
        {
            throw new NotImplementedException();
        }
    }
}
