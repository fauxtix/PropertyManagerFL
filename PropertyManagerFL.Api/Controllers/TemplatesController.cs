using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using System.IO;

namespace PropertyManagerFL.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TemplatesController : ControllerBase
{
    private readonly ILetterTemplatesRepository _templateRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<TemplatesController> _logger;


    public TemplatesController(ILetterTemplatesRepository templateRepository, IWebHostEnvironment webHostEnvironment, ILogger<TemplatesController> logger)
    {
        _templateRepository = templateRepository;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTemplates()
    {
        try
        {
            var templates = await _templateRepository.GetAllTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplateById(int id)
    {
        try
        {
            var template = await _templateRepository.GetTemplateByIdAsync(id);

            if (template == null)
            {
                _logger.LogWarning($"Templates Api - template com o Id {id} não encontrado)");
                return NotFound();
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("GetTemplateName/{templateName}")]
    public string GetTemplate(string templateName)
    {
        try
        {

            var fileLocation = Path.Combine(_webHostEnvironment.ContentRootPath, "reports", "docs", templateName);
            if (!System.IO.File.Exists(fileLocation))
            {
                _logger.LogWarning($"Templates Api - ficheiro ({templateName}) não encontrado)");
                return "";
            }

            return fileLocation;

            //var fileBytes = System.IO.File.ReadAllBytes(fileLocation);
            //return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileLocation);
            //var stream = new FileStream(fileLocation, FileMode.Open);
            //return File(stream, "application/pdf", templateName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return "";
        }
    }

    [HttpGet("GetTemplatesFilenamesFromServer/{culture}")]
    public List<string> GetTemplatesFilenamesFromServer(string culture)
    {
        try
        {

            var templatesLocation = Path.Combine(_webHostEnvironment.ContentRootPath, "reports", "docs");
            string[] files = Directory.GetFiles(templatesLocation, "*.dotx");
            if (files.Length == 0)
                return new List<string>();

            List<string> result = FilterFilesByCulture(files, culture);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new List<string>();
        }
    }



    [HttpPost]
    public async Task<IActionResult> AddTemplate([FromBody] Template template)
    {
        try
        {
            var insertedId = await _templateRepository.InsertAsync(template);
            return Ok(insertedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(int id, [FromBody] Template template)
    {
        try
        {
            if (id != template.Id)
            {
                _logger.LogWarning("Templates API - Mismatched IDs");
                return BadRequest("Mismatched IDs");
            }

            var success = await _templateRepository.UpdateAsync(template);

            if (success)
            {
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("Templates API - Erro ao atualizar template. Verifique, p.f.");

                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    private List<string> FilterFilesByCulture(string[] files, string culture)
    {
        List<string> result = new List<string>();

        if (files.Length == 0)
            return result;

        string cultureKeyword = GetCultureKeyword(culture);

        if (string.IsNullOrEmpty(cultureKeyword))
            result.AddRange(files);
        else
        {
            result.AddRange(files.Where(f => f.ToLower().Contains(cultureKeyword)));
            if (result.Count == 0)
            {
                return files.ToList();
            }
        }

        return result;
    }

    private string GetCultureKeyword(string culture)
    {
        if (culture.Contains("es"))
            return "espanhol";
        else if (culture.Contains("fr"))
            return "frances";
        else if (culture.Contains("en"))
            return "ingles";
        else if (culture.Contains("pt"))
            return "portugues";
        else
            return "";
    }
}
