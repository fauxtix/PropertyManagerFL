using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetAllMessages();
        Task<Message> GetMessageById(int id);
        Task<int >Add(Message message);
        Task<bool> Save(Message message);
        Task<bool> Delete(int Id);
    }
}
