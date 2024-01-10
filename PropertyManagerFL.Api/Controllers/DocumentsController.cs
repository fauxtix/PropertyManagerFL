using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Documentos;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsRepository _repoDocuments;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IWebHostEnvironment _environment;


        public DocumentsController(IMapper mapper,
                                       ILogger<DocumentsController> logger,
                                       IWebHostEnvironment environment,
                                       IDocumentsRepository repoDocuments)
        {
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
            _repoDocuments = repoDocuments;
        }


        /// <summary>
        /// Novo Documento
        /// </summary>
        /// <param name="mewDocument"></param>
        /// <returns></returns>
        [HttpPost("InsertDocument")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsertDocument([FromBody] NovoDocumento mewDocument)
        {
            var location = GetControllerActionNames();
            try
            {
                if (mewDocument is null)
                {
                    return BadRequest();
                }
                int insertedId = await _repoDocuments.InsertDocument(mewDocument);
                var viewDocument = await _repoDocuments.GetDocument_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetDocumentById), new { id = viewDocument.Id }, viewDocument);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Atualização de Documento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDocument"></param>
        /// <returns></returns>
        [HttpPut("UpdateDocument/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateDocument(int id, [FromBody] AlteraDocumento updateDocument)
        {
            var location = GetControllerActionNames();
            try
            {
                if (updateDocument == null)
                {
                    string? msg = "O Documento passado como paràmetro é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != updateDocument.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var documentToUpdate = await _repoDocuments.GetDocument_ById(id);

                if (documentToUpdate == null)
                {
                    return NotFound($"O Documento com o Id {id} não foi encontrado");
                }

                var updateddocument = await _repoDocuments.UpdateDocument(updateDocument);
                if (updateddocument is null)
                    return BadRequest("document not updated");

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Apaga Documento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteDocument/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var documentToDelete = await _repoDocuments.GetDocument_ById(id);
                if (documentToDelete == null)
                {
                    _logger.LogError($"Documento com o Id {id} não encontrado");
                    return NotFound($"Documento com o Id {id} não encontrado");
                }

                await _repoDocuments.DeleteDocument(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("GetDocuments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDocuments()
        {
            var location = GetControllerActionNames();
            try
            {
                var documents = await _repoDocuments.GetAll();
                if (documents.Any())
                {
                    return Ok(documents);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetDocumentTypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDocumentTypes()
        {
            var location = GetControllerActionNames();
            try
            {
                var documents = await _repoDocuments.GetAll_DocumentTypes();
                if (documents is not null)
                {
                    return Ok(documents);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Pesquisa Documento por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetDocument_ById/{id:int}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var document = await _repoDocuments.GetDocument_ById(id);
                if (document is not null)
                    return Ok(document);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetDocumentType_ById/{id:int}")]
        public async Task<IActionResult> GetDocumentType_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var document = await _repoDocuments.GetDocumentType_ById(id);
                if (document is not null)
                    return Ok(document);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }



        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, $"Algo de errado ocorreu ({message}). contacte o Administrador");
        }
    }
}
