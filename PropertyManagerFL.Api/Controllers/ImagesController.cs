using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Infrastructure.Security;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IWebHostEnvironment environment, ILogger<ImagesController> logger)
        {
            this.environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Guarda imagem no servidor
        /// </summary>
        /// <returns></returns>
        [HttpPost("Save")]
        public async Task<string> Save()
        {
            string path = string.Empty;
            if (HttpContext.Request.Form.Files.Any())
            {
                var root = Path.Combine(environment.ContentRootPath, "uploads");
                Directory.CreateDirectory(root);

                foreach (var file in HttpContext.Request.Form.Files)
                {
                    var fileName = SafePath.FileName(file.FileName);
                    path = SafePath.CombineContained(root, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            byte[] ByteArray = System.IO.File.ReadAllBytes(path);

            return Convert.ToBase64String(ByteArray);
        }

        /// <summary>
        /// Save image
        /// </summary>
        /// <param name="arquivo"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<string> EnviaArquivo([FromForm] IFormFile arquivo)
        {
            if (arquivo.Length > 0)
            {
                try
                {
                    var root = Path.Combine(environment.WebRootPath ?? environment.ContentRootPath, "uploads");
                    Directory.CreateDirectory(root);

                    var fileName = SafePath.FileName(arquivo.FileName);
                    var path = SafePath.CombineContained(root, fileName);

                    using (FileStream filestream = System.IO.File.Create(path))
                    {
                        await arquivo.CopyToAsync(filestream);
                        await filestream.FlushAsync();
                        return "\\uploads\\" + fileName;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao guardar imagem");
                    return "Ocorreu uma falha no envio do arquivo...";
                }
            }
            else
            {
                return "Ocorreu uma falha no envio do arquivo...";
            }
        }
    }
}
