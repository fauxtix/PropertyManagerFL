using PropertyManagerFL.Application.Interfaces.Chat;
using PropertyManagerFL.Application.Models.Chat;
using PropertyManagerFL.Application.Responses.Identity;
using PropertyManagerFL.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Communication
{
    public interface IChatManager : IManager
    {
        Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
    }
}