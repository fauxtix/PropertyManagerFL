using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Documentos;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IDocumentsRepository
    {
        Task DeleteDocument(int id);
        Task<IEnumerable<DocumentoVM>> GetAll();
        Task<DocumentoVM> GetDocument_ById(int id);
        Task<int> InsertDocument(NovoDocumento newDocument);
        Task<DocumentoVM?> UpdateDocument(AlteraDocumento updateDocument);
        Task<IEnumerable<DocumentType>> GetAll_DocumentTypes();
        Task<DocumentType> GetDocumentType_ById(int id);
    }
}