using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IHelpManagerRepository : IBaseRepository<HelpIndex>
    {
        HelpViewModel GetHelpData(int IdProjeto, string NomeForm);
        int GetIdProjeto(string sProjName);
        bool HelpExists(int IdParent, string NomeForm);
    }
}