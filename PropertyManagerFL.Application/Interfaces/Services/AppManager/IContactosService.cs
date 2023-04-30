using PropertyManagerFL.Application.ViewModels.Contactos;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface IContactosService
    {
        Task<bool> InsereContacto(ContactoVM contacto);
        Task<bool> AtualizaContacto(int id, ContactoVM contacto);
        Task<bool> ApagaContacto(int id);
        Task<ContactoVM> GetContacto_ById(int id);
        Task<IEnumerable<ContactoVM>> GetAll();
    }
}