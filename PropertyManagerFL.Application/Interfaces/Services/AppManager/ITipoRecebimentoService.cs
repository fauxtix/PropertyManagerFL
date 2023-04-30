using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface ITipoRecebimentoService
    {
        void Delete(TipoRecebimento entity);
        int GetFirstId();
        long Insert(TipoRecebimento entity);
        IEnumerable<TipoRecebimento> Query(string where = null);
        TipoRecebimento Query_ById(int id);
        string RegistoComErros(TipoRecebimento tipoRecebimento);
        bool TableHasData();
        void Update(TipoRecebimento entity);
        int GetID_ByDescription(string sDescricao);
    }
}