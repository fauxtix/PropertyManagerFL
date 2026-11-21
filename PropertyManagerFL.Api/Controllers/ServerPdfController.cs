using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Infrastructure.Security;

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
            var fileLocation = BuildPath(folder, filename);

            var stream = new FileStream(fileLocation, FileMode.Open);
            return File(stream, "application/pdf", SafePath.FileName(filename));

        }

        [HttpGet("GetServerPdfName/{folder}/{filename}")]
        public string GetFileName(string folder, string filename)
        {
            return BuildPath(folder, filename);
        }

        private string BuildPath(string folder, string filename)
        {
            var root = Path.Combine(_webHostEnvironment.ContentRootPath, "reports", "docs");
            return SafePath.CombineContained(root, SafePath.FileName(folder), SafePath.FileName(filename));
        }

    }
}
