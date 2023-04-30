using PropertyManagerFL.Application.ViewModels.Messages;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface IMessagesService
    {
        Task<IEnumerable<ComposeMessageVM>> GetAllMessages();
        Task<ComposeMessageVM> GetMessageById(int id);
        Task<bool> Add(ComposeMessageVM message);
        Task<bool> Save(int Id, ComposeMessageVM message);
        Task<bool> Delete(int Id);
    }
}
