using PropertyManagerFL.Application.Features.Products.Commands.AddEdit;
using PropertyManagerFL.Application.Features.Products.Queries.GetAllPaged;
using PropertyManagerFL.Application.Requests.Catalog;
using PropertyManagerFL.Shared.Wrapper;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Catalog.Product
{
    public interface IProductManager : IManager
    {
        Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request);

        Task<IResult<string>> GetProductImageAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditProductCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}