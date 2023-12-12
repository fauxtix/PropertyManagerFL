using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface ITipoPropriedadeService
    {
        void Delete(TipoPropriedade entity);
        int GetFirstId();
        long Insert(TipoPropriedade entity);
        IEnumerable<TipoPropriedade> Query(string where = "");
        TipoPropriedade Query_ById(int id);
        string RegistoComErros(TipoPropriedade tipoPropriedade);
        bool TableHasData();
        void Update(TipoPropriedade entity);
    }
}