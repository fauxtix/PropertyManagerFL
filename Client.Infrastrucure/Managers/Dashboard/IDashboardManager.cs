using PropertyManagerFL.Application.Features.Dashboards.Queries.GetData;
using PropertyManagerFL.Shared.Wrapper;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();
    }
}