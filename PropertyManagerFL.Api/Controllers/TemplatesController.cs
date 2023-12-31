using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Infrastructure.Repositories;

namespace PropertyManagerFL.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TemplatesController : ControllerBase
{
    private readonly ILetterTemplatesRepository _templateRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public TemplatesController(ILetterTemplatesRepository templateRepository, IWebHostEnvironment webHostEnvironment)
    {
        _templateRepository = templateRepository;
        _webHostEnvironment = webHostEnvironment;
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
            // Log the exception or handle it appropriately
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
                return NotFound();
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
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
            // Log the exception or handle it appropriately
            return "";
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
            // Log the exception or handle it appropriately
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
                return BadRequest("Mismatched IDs");
            }

            var success = await _templateRepository.UpdateAsync(template);

            if (success)
            {
                return Ok(true);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            return StatusCode(500, "Internal Server Error");
        }
    }


}
