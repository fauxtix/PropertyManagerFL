using AutoMapper;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Documentos;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperDocumentos : IDocumentosService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperDocumentos> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WrapperDocumentos(IConfiguration env, ILogger<WrapperDocumentos> logger, HttpClient httpClient, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Documents";
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> InsertDocument(DocumentoVM document)
        {
            try
            {
                var documentToInsert = _mapper.Map<NovoDocumento>(document);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsertDocument", documentToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar documento {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDocument(int id, DocumentoVM document)
        {
            try
            {
                var documentToUpdate = _mapper.Map<AlteraDocumento>(document);

                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/UpdateDocument/{id}", documentToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar documento");
                return false;
            }
        }

        /// <summary>
        /// Delete document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDocument(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/DeleteDocument/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar documento)");
                return false;
            }
        }

        /// <summary>
        /// Get all documents
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DocumentoVM>> GetAll()
        {
            try
            {
                var documents = await _httpClient.GetFromJsonAsync<IEnumerable<DocumentoVM>>($"{_uri}/GetDocuments");
                return documents!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Documentos/GetAll)");
                return Enumerable.Empty<DocumentoVM>();
            }
        }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DocumentoVM> GetDocument_ById(int id)
        {
            try
            {
                var document = await _httpClient.GetFromJsonAsync<DocumentoVM>($"{_uri}/GetDocument_ById/{id}");
                return document!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Documentos/GetDocument_ById)");
                return new DocumentoVM();
            }
        }

        public async Task<IEnumerable<DocumentType>> GetAll_DocumentTypes()
        {
            try
            {
                var documents = await _httpClient.GetFromJsonAsync<IEnumerable<DocumentType>>($"{_uri}/GetDocumentTypes");
                return documents!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Documentos/GetAll_DocumentTypes)");
                return Enumerable.Empty<DocumentType>();
            }
        }

        public async Task<DocumentType> GetDocumentType_ById(int id)
        {
            try
            {
                var document = await _httpClient.GetFromJsonAsync<DocumentType>($"{_uri}/GetDocument_ById/{id}");
                return document!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Documentos/GetDocumentType_ById)");
                return new DocumentType();
            }
        }

        public string GetPdfFilename(string pasta, string filename)
        {
            try
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", pasta, filename);
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Documentos/GetPdfFilename)");
                return "";
            }
        }
    }
}
