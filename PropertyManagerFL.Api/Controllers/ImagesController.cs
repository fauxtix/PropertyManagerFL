using Microsoft.AspNetCore.Mvc;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        public ImagesController(IWebHostEnvironment environment)
        {
            this.environment = environment;
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
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    path = Path.Combine(environment.ContentRootPath, "uploads", file.FileName);
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
                    if (!Directory.Exists(environment.WebRootPath + "\\uploads\\"))
                    {
                        Directory.CreateDirectory(environment.WebRootPath + "\\uploads\\");
                    }
                    using (FileStream filestream = System.IO.File.Create(environment.WebRootPath + "\\uploads\\" + arquivo.FileName))
                    {
                        await arquivo.CopyToAsync(filestream);
                        filestream.Flush();
                        return "\\uploads\\" + arquivo.FileName;
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            else
            {
                return "Ocorreu uma falha no envio do arquivo...";
            }
        }
    }
}
