using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Documentos;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ContactRepository> _logger;

        public DocumentsRepository(DapperContext context, ILogger<ContactRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsertDocument(NovoDocumento newDocument)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Documents_Insert",
                         param: newDocument, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<DocumentoVM?> UpdateDocument(AlteraDocumento updateDocument)
        {
            updateDocument.LastModifiedBy = "Fausto";
            updateDocument.LastModifiedOn = DateTime.Now;

            var parameters = new DynamicParameters();
            parameters.Add("@Id", updateDocument.Id);
            parameters.Add("@Title", updateDocument.Title);
            parameters.Add("@Description", updateDocument.Description);
            parameters.Add("@URL", updateDocument.URL);
            parameters.Add("@LocalUpload", updateDocument.LocalUpload);
            parameters.Add("@IsPublic", updateDocument.IsPublic );
            parameters.Add("@LastModifiedBy", updateDocument.LastModifiedBy);
            parameters.Add("@LastModifiedOn", updateDocument.LastModifiedOn);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var document = await connection.QueryFirstAsync<DocumentoVM>("usp_Documents_Update",
                         param: parameters, 
                         commandType: CommandType.StoredProcedure);

                    return document;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task DeleteDocument(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Documents_Delete",
                    param: parameters, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public async Task<IEnumerable<DocumentoVM>> GetAll()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<DocumentoVM>("usp_Documents_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        public async Task<IEnumerable<DocumentType>> GetAll_DocumentTypes()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<DocumentType>("usp_DocumentTypes_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }


        public async Task<DocumentoVM> GetDocument_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<DocumentoVM>("usp_Documents_GetById",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        public async Task<DocumentType> GetDocumentType_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<DocumentType>("usp_DocumentTypes_GetById",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

    }
}
