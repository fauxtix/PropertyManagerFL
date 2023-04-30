using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.Application.Interfaces.Services.Common
{
    public interface iHelpManagerService
    {
        HelpViewModel GetHelpData(int IdProjeto, string NomeForm);
        int GetIdProjeto(string sProjName);
        bool HelpExists(int IdParent, string NomeForm);
    }
}
