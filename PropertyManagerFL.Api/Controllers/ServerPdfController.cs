using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerPdfController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ServerPdfController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("Download/{folder}/{filename}"), DisableRequestSizeLimit]
        public IActionResult Download(string folder, string filename)
        {
            var fileLocation = Path.Combine(_webHostEnvironment.ContentRootPath, "reports", "docs", folder, filename);

            var stream = new FileStream(fileLocation, FileMode.Open);
            return File(stream, "application/pdf", filename);

        }

        [HttpGet("GetServerPdfName/{folder}/{filename}")]
        public string GetFileName(string folder, string filename)
        {
            var fileLocation = Path.Combine(_webHostEnvironment.ContentRootPath, "reports", "docs", folder, filename);

            return fileLocation;
        }

    }
}
