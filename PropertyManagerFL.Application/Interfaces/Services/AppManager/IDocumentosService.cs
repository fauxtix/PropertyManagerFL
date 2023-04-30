using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Documentos;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface IDocumentosService
    {
        Task<bool> DeleteDocument(int id);
        Task<IEnumerable<DocumentoVM>> GetAll();
        Task<DocumentoVM> GetDocument_ById(int id);
        Task<bool> InsertDocument(DocumentoVM newDocument);
        Task<bool> UpdateDocument(int id, DocumentoVM updateDocument);
        Task<IEnumerable<DocumentType>> GetAll_DocumentTypes();
        Task<DocumentType> GetDocumentType_ById(int id);
        string GetPdfFilename(string pasta, string filename);
    }
}
