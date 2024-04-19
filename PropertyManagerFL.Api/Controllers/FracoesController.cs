using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Controller de Fracoes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]


    public class FracoesController : ControllerBase
    {

        private readonly IFracaoRepository _repoFracoes;
        private readonly IMapper _mapper;
        private readonly ILogger<FracoesController> _logger;

        /// <summary>
        /// Construtor de frações
        /// </summary>
        /// <param name="repoFracoes"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public FracoesController(IFracaoRepository repoFracoes, ILogger<FracoesController> logger, IMapper mapper)
        {
            _repoFracoes = repoFracoes;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria nova fração
        /// </summary>
        /// <param name="fracao"></param>
        /// <returns></returns>

        [HttpPost("InsereFracao")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereFracao([FromBody] FracaoVM fracao)
        {
            var location = GetControllerActionNames();
            try
            {
                if (fracao is null)
                {
                    return BadRequest();
                }
                var imagensFracao = fracao.Imagens;

                var novaFracao = _mapper.Map<NovaFracao>(fracao);
                var newPolicy = _mapper.Map<Seguro>(fracao.Apolice);

                int idUnitCreated = await _repoFracoes.InsereFracao(novaFracao, imagensFracao!, newPolicy);
                if (idUnitCreated > 0)
                {
                    var actionReturned = CreatedAtAction(nameof(GetFracaoById), new { id = idUnitCreated }, fracao);
                    return actionReturned;
                }
                else
                {
                    return InternalError($"{location}: Erro no Api (InsereFracao");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpPost("InsereImagemFracao")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereImagemFracao([FromBody] NovaImagemFracao imagem)
        {
            var location = GetControllerActionNames();
            try
            {
                if (imagem is null)
                {
                    return BadRequest();
                }

                int idUnitCreated = await _repoFracoes.InsereImagemFracao(imagem);
                if (idUnitCreated > 0)
                {
                    var actionReturned = CreatedAtAction(nameof(GetImages_ByUnitId), new { id = idUnitCreated }, imagem);
                    return actionReturned;
                }
                else
                {
                    return InternalError($"{location}: Erro no Api (InsereImagemFracao");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Cria nova apólice da fração
        /// </summary>
        /// <param name="apolice"></param>
        /// <returns></returns>

        [HttpPost("InsereApoliceFracao")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereApoliceFracao([FromBody] SeguroVM apolice)
        {
            var location = GetControllerActionNames();
            try
            {
                if (apolice is null)
                {
                    return BadRequest();
                }

                var novaApolice = _mapper.Map<Seguro>(apolice);

                int idApoliceCreated = await _repoFracoes.InsereApoliceFracao(novaApolice);
                if (idApoliceCreated > 0)
                {
                    var actionReturned = CreatedAtAction(nameof(GetApoliceById), new { id = idApoliceCreated }, apolice);
                    return actionReturned;
                }
                else
                {
                    return InternalError($"{location}: Erro no Api (InsereApolice");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Cria nova apólice da fração
        /// </summary>
        /// <param name="seguro"></param>
        /// <returns></returns>

        /// <summary>
        /// Altera dados de fração
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <param name="updatedUnit"></param>
        /// <returns></returns>
        ///    [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [HttpPut("AlteraFracao/{id:int}")]
        public async Task<IActionResult> AlteraFracao(int id, [FromBody] FracaoVM updatedUnit)
        {
            var location = GetControllerActionNames();
            try
            {
                if (updatedUnit == null)
                {
                    string msg = "A fração passada como paràmetro, é incorreta.";
                    _logger.LogWarning("A fração passada como paràmetro, é incorreta.");
                    return BadRequest(msg);
                }

                if (id != updatedUnit.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var dbUnit = await _repoFracoes.GetFracao_ById(id);

                if (dbUnit == null)
                {
                    return NotFound($"A fração com o Id {id} não foi encontrada");
                }

                var imagensFracao = updatedUnit.Imagens;

                var unitToUpdate = _mapper.Map<AlteraFracao>(updatedUnit);
                var policyToUpdate = _mapper.Map<Seguro>(updatedUnit.Apolice);
                var fracaoAlterada = await _repoFracoes.AtualizaFracao(unitToUpdate, imagensFracao, policyToUpdate);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [HttpPut("AlteraImagemFracao/{id:int}")]
        public async Task<IActionResult> AlteraImagemFracao(int id, [FromBody] AlteraImagemFracao updatedImage)
        {
            var location = GetControllerActionNames();
            try
            {
                if (updatedImage == null)
                {
                    string msg = "A imagem passada como paràmetro, é incorreta.";
                    _logger.LogWarning("A imagem passada como paràmetro, é incorreta.");
                    return BadRequest(msg);
                }

                if (id != updatedImage.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var dbImage = await _repoFracoes.GetImage_ByUnitId(id);

                if (dbImage == null)
                {
                    return NotFound($"A imagem com o Id {id} não foi encontrada");
                }

                var fracaoAlterada = await _repoFracoes.AtualizaImagemFracao(updatedImage);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Apaga fração
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("ApagaFracao/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaFracao(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var fracaoToDelete = await _repoFracoes.GetFracao_ById(id);
                if (fracaoToDelete == null)
                {
                    _logger.LogError($"Fracão com o Id {id} não encontrada");
                    return NotFound($"Fração com o Id {id} não encontrada");
                }

                await _repoFracoes.ApagaFracao(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpDelete("ApagaImagemFracao/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaImagemFracao(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var imagemToDelete = await _repoFracoes.GetImages_ByUnitId(id);
                if (imagemToDelete == null)
                {
                    _logger.LogError($"Imagem com o Id {id} não encontrada");
                    return NotFound($"Imagem com o Id {id} não encontrada");
                }

                await _repoFracoes.ApagaImagemFracao(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }


        /// <summary>
        /// Get all Frações
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFracoes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFracoes()
        {
            var location = GetControllerActionNames();
            try
            {
                var fracoes = await _repoFracoes.GetAll();
                if (fracoes.Any())
                {
                    return Ok(fracoes);
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

        [HttpGet("GetFracoesDisponiveis/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFracoesDisponiveis(int id = 0)
        {
            var location = GetControllerActionNames();
            try
            {
                var unitsAvailable = await _repoFracoes.GetFracoes_Disponiveis(id);
                if (unitsAvailable.Count() >= 0)
                {
                    return Ok(unitsAvailable);
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

        [HttpGet("UnitsWithDuePayments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UnitsWithDuePayments()
        {
            var location = GetControllerActionNames();
            try
            {
                var unitsAvailable = await _repoFracoes.GetFracoes_WithDuePayments();
                if (unitsAvailable is not null)
                {
                    return Ok(unitsAvailable);
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

        [HttpGet("GetFracoesSemContrato/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFracoesSemContrato(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var unitsAvailable = await _repoFracoes.GetFracoes_SemContrato(id);
                if (unitsAvailable.Count() >= 0)
                {
                    return Ok(unitsAvailable);
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
        /// Frações de um imóvel (para preencher combo) - repetida com 'GetFracoesById'
        /// </summary>
        /// <param name="id">Id do imóvel</param>
        /// <returns></returns>
        [HttpGet("GetFracoesLookup/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFracoesLookup(int id = 0)
        {
            var location = GetControllerActionNames();
            try
            {
                var fracoesLookup = await _repoFracoes.GetFracoes(id);
                if (fracoesLookup.Any())
                {
                    return Ok(fracoesLookup);
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
        /// Devolve frações de um imóvel
        /// </summary>
        /// <param name="id">Id do imóvel</param>
        /// <returns>Lista de frações</returns>
        [HttpGet("GetFracoes_Imovel/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFracoes_Imovel(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }
                var fracoes_Imovel = await _repoFracoes.GetFracoes_Imovel(id);
                if (fracoes_Imovel.Any())
                {
                    return Ok(fracoes_Imovel);
                }
                else
                {
                    return NotFound($"Imóvel {id} não tem inserida qualquer fração");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Fração por Id
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <returns>View Model</returns>
        [HttpGet("GetFracaoById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFracaoById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var fracao = await _repoFracoes.GetFracao_ById(id);
                if (fracao is not null)
                {
                    return Ok(fracao);
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
        /// Apólice por Id
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <returns>View Model</returns>
        [HttpGet("GetApoliceById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetApoliceById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var apolice = await _repoFracoes.GetApoliceFracao_ById(id);
                if (apolice is not null)
                {
                    return Ok(apolice);
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
        /// Fração por Id
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <returns>Fração</returns>
        [HttpGet("GetUnitById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetUnitById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var unit = await _repoFracoes.GetUnit_ById(id);
                if (unit is not null)
                {
                    return Ok(unit);
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


        [HttpGet("GetIDSituacao_ByDescription/{description}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetIDSituacao_ByDescription(string description)
        {
            var location = GetControllerActionNames();
            try
            {
                if (string.IsNullOrEmpty(description))
                {
                    return BadRequest();
                }

                var unitStatusId = await _repoFracoes.GetIDSituacao_ByDescription(description);
                if (unitStatusId > 0)
                {
                    return Ok(unitStatusId);
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

        [HttpGet("GetImages_ByUnitId/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetImages_ByUnitId(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var images = await _repoFracoes.GetImages_ByUnitId(id);
                if (images is not null)
                {
                    return Ok(images);
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

        [HttpGet("GetImage_ByUnitId/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetImage_ByUnitId(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var image = await _repoFracoes.GetImage_ByUnitId(id);
                if (image is not null)
                {
                    return Ok(image);
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

        [HttpGet("GetUnits_Insurance_Data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetUnits_Insurance_Data()
        {
            var location = GetControllerActionNames();
            try
            {

                var insuranceData = await _repoFracoes.GetUnitsInsuranceData();
                if (insuranceData is not null)
                {
                    return Ok(insuranceData);
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


        [HttpGet("IsUnitFreeToLease/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> IsUnitFreeToLease(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var freeForTransaction = await _repoFracoes.FracaoEstaLivre(id);
                if (freeForTransaction)
                {
                    return Ok(freeForTransaction);
                }
                else
                {
                    return NotFound($"Fração {id} não encontrada");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("SetUnitAsRented/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> SetUnitAsRented(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var freeUnit = await _repoFracoes.MarcaFracaoComoAlugada(id);
                return Ok(freeUnit);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("UpdateRentValue/{id:int}/{newRentValue}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateRentValue(int id, string newRentValue)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var rec = await _repoFracoes.GetFracao_ById(id);
                if (rec is null)
                {
                    return NotFound("Id não encontrado");
                }

                if (!DataFormat.IsNumeric(newRentValue))
                {
                    return BadRequest("Valor a atualizar inválido");
                }

                var parsedValue = DataFormat.DecimalParse(newRentValue);

                var updateRentOk = await _repoFracoes.AtualizaValorRenda(id, parsedValue);
                return Ok(updateRentOk);
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
            return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
        }
    }
}
