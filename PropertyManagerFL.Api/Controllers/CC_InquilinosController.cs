using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Tenants CC  Api
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class CC_InquilinosController : ControllerBase
    {
        private readonly ICC_InquilinoRepository _repoCC_Inquilinos;
        private readonly IMapper _mapper;
        private readonly ILogger<CC_InquilinosController> _logger;

        /// <summary>
        /// Controller de Inquilinos
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="repoCC_Inquilinos"></param>
		public CC_InquilinosController(IMapper mapper, ILogger<CC_InquilinosController> logger, ICC_InquilinoRepository repoCC_Inquilinos)
        {
            _mapper = mapper;
            this._logger = logger;
            _repoCC_Inquilinos = repoCC_Inquilinos;
        }

        /// <summary>
        /// Returns all transactions
        /// </summary>
        /// <returns>IEnumerable Inquilino</returns>
        // GET: api/<InquilinosController>
        [HttpGet("AllTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAllTransactions()
        {
            var location = GetControllerActionNames();
            try
            {
                var transactions = await _repoCC_Inquilinos.GetAll();
                if (transactions.Any())
                {
                    return Ok(transactions);
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
        /// Get transaction by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetTransationById/{id:int}")]
        public async Task<IActionResult> GetTransationById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var transation = await _repoCC_Inquilinos.Query_ById(id);
                if (transation is not null)
                    return Ok(transation);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get primeiro Id da transação
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFirstTransactionId")]
        public async Task<IActionResult> GetFirstTransactionId()
        {
            var location = GetControllerActionNames();
            try
            {
                var transactionId = await _repoCC_Inquilinos.GetFirstId();
                if (transactionId > 0)
                    return Ok(transactionId);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }




        /// <summary>
        /// Insert new transaction
        /// </summary>
        /// <param name="newTransaction"></param>
        /// <returns>url to access the tenant created</returns>
        [HttpPost("InsertTransaction")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsertTransaction([FromBody] CC_InquilinoNovo newTransaction)
        {
            var location = GetControllerActionNames();
            try
            {
                if (newTransaction is null)
                {
                    return BadRequest();
                }
                var insertedId = await _repoCC_Inquilinos.Insert(newTransaction);
                var viewTransaction = _repoCC_Inquilinos.Insert(newTransaction);
                var actionReturned = CreatedAtAction(nameof(GetTransationById), new { id = viewTransaction.Id }, viewTransaction);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Update transaction
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateTransaction"></param>
        /// <returns></returns>
        [HttpPut("UpdateTransaction/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraInquilino(int id, [FromBody] CC_InquilinoAltera updateTransaction)
        {
            var location = GetControllerActionNames();
            try
            {
                if (updateTransaction == null)
                {
                    string msg = "Transação incorreta.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != updateTransaction.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var transactionToUpdate = await _repoCC_Inquilinos.Query_ById(id);

                if (transactionToUpdate == null)
                {
                    return NotFound($"Transação com o Id {id} não foi encontrada");
                }

                var inquilinoAlterado = await _repoCC_Inquilinos.Update(updateTransaction);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }



        /// <summary>
        /// Delete transaction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpDelete("DeleteTransaction/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var transactionToDelete = await _repoCC_Inquilinos.Query_ById(id);
                if (transactionToDelete == null)
                {
                    _logger.LogError($"Transação com o Id {id} não encontrada");
                    return NotFound($"Transação com o Id {id} não encontrada");
                }


                await _repoCC_Inquilinos.Delete(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
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
            return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
        }
    }
}
