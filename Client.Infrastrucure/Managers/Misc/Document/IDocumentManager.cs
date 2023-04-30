using PropertyManagerFL.Application.Features.Documents.Commands.AddEdit;
using PropertyManagerFL.Application.Features.Documents.Queries.GetAll;
using PropertyManagerFL.Application.Features.Documents.Queries.GetById;
using PropertyManagerFL.Application.Requests.Documents;
using PropertyManagerFL.Shared.Wrapper;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Misc.Document
{
    public interface IDocumentManager : IManager
    {
        Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

        Task<IResult<int>> SaveAsync(AddEditDocumentCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}