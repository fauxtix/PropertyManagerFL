using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface ITipoContactoService
    {
        void Delete(TipoContacto entity);
        int GetFirstId();
        long Insert(TipoContacto entity);
        IEnumerable<TipoContacto> Query(string where = null);
        TipoContacto Query_ById(int id);
        string RegistoComErros(TipoContacto tipoContacto);
        bool TableHasData();
        void Update(TipoContacto entity);
    }
}