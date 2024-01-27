using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Core.Entities;
using System.Diagnostics.Metrics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PropertyManagerFL.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HistoricoEnvioCartasController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger<ImoveisController> _logger;
    private readonly IHistoricoEnvioCartasRepository _repoLetters;

    public HistoricoEnvioCartasController(IHistoricoEnvioCartasRepository repoLetters, IMapper mapper, ILogger<ImoveisController> logger)
    {
        _repoLetters = repoLetters;
        _mapper = mapper;
        _logger = logger;
    }


    /// <summary>
    /// Cria novo imóvel
    /// </summary>
    /// <param name="letterSent"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> InsertInHistory([FromBody] HistoricoEnvioCartasVM letterSent)
    {
        var location = GetControllerActionNames();
        try
        {
            if (letterSent is null)
            {
                return BadRequest();
            }

            var lettertype = (int)letterSent.IdTipoCarta;
            var letterSentEntity = _mapper.Map<HistoricoEnvioCartas>(letterSent);
            var insertedId = await _repoLetters.InsertLetterSent(letterSentEntity);
            var insertedLetterSent= await _repoLetters.GetLetterSent(insertedId);

            var actionReturned = CreatedAtAction(nameof(GetLetterSent), new { id = letterSent.Id }, insertedLetterSent);
            return actionReturned;
        }
        catch (Exception e)
        {
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }

    }

    /// <summary>
    /// Data de resposta de carta enviada
    /// </summary>
    /// <param name="id"></param>
    /// <param name="answerDate"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> UpdateLetterSentAnswerDate(int id, DateTime answerDate )
    {
        var location = GetControllerActionNames();
        try
        {
            if (id < 1)
            {
                string msg = "O Id passado como paràmetro, é incorreto.";
                _logger.LogWarning(msg);
                return BadRequest(msg);
            }
            if(!DataFormat.IsValidDate(answerDate))
            {
                return BadRequest("Data inválida");
            }
            else
            {
                if(answerDate > DateTime.Now)
                {
                    return BadRequest("Data inválida");
                }
            }

            var recordToUpdate = await _repoLetters.GetLetterSent(id);
            if (recordToUpdate is null)
            {
                return NotFound("Registo a atualizar não foi encontrado");
            }


            var updatedRecord = await _repoLetters.UpdateLetterAnsweredDate(id, answerDate);
            return Ok();
        }
        catch (Exception e)
        {
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }


    /// <summary>
    ///  Get letter sent
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetLettersSent()
    {
        var location = GetControllerActionNames();
        try
        {
            var letters = await _repoLetters.GetLettersSent();
            if (letters.Any())
            {
                return Ok(letters);
            }
            else
            {
                return NotFound("Sem registos no histórico");
            }
        }
        catch (Exception e)
        {
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    /// <summary>
    ///  Get letter sent
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetLetterSent(int Id)
    {
        var location = GetControllerActionNames();
        try
        {
            var letter = await _repoLetters.GetLetterSent(Id);
            if (letter is not null)
            {
                return Ok(letter);
            }
            else
            {
                return NotFound("Registo não encontrado");
            }
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
