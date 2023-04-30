using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Contactos;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IContactRepository
    {
        Task<int> InsereContacto(NovoContacto novoContacto);
        Task<ContactoVM?> AtualizaContacto(AlteraContacto alteraContacto);
        Task ApagaContacto(int id);
        Task<ContactoVM> GetContacto_ById(int id);
        Task<IEnumerable<ContactoVM>> GetAll();
    }
}
