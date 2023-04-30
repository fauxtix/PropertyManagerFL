using PropertyManagerFL.Application.Features.DocumentTypes.Commands.AddEdit;
using PropertyManagerFL.Application.Features.DocumentTypes.Queries.GetAll;
using PropertyManagerFL.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Misc.DocumentType
{
    public interface IDocumentTypeManager : IManager
    {
        Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}