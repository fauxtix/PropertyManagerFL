using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface ICategoriaDespesaService
    {
        void Delete(CategoriaDespesa entity);
        int GetFirstId();
        long Insert(CategoriaDespesa entity);
        IEnumerable<CategoriaDespesa> Query(string where = null);
        CategoriaDespesa Query_ById(int id);
        string RegistoComErros(CategoriaDespesa categoriasDespesas);
        bool TableHasData();
        void Update(CategoriaDespesa entity);
    }
}