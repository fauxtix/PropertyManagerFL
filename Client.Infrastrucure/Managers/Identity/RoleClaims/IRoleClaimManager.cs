using PropertyManagerFL.Application.Requests.Identity;
using PropertyManagerFL.Application.Responses.Identity;
using PropertyManagerFL.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Identity.RoleClaims
{
    public interface IRoleClaimManager : IManager
    {
        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsAsync();

        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId);

        Task<IResult<string>> SaveAsync(RoleClaimRequest role);

        Task<IResult<string>> DeleteAsync(string id);
    }
}