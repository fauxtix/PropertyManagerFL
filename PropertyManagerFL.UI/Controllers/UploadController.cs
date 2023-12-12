using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace PropertyManagerFL.UI.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UploadController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        [HttpPost("[action]/{folder}")]
        public void Save(List<IFormFile> uploadFiles, string folder)
        {
            long size = 0;
            try
            {
                foreach (var file in uploadFiles)
                {
                    var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    if (contentDisposition != null)
                    {
                        var fileName = contentDisposition.FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var filename = fileName.Trim('"');
                            var filenameToCopy = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", folder, filename);
                            size += file.Length;

                            if (!System.IO.File.Exists(filenameToCopy))
                            {
                                using (var fs = System.IO.File.Create(filenameToCopy))
                                {
                                    file.CopyTo(fs);
                                    fs.Flush();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "File failed to upload");
            }
        }

        [HttpPost("[action]")]
        public void Remove(List<IFormFile> uploadFiles)
        {
            try
            {
                if (uploadFiles.Count > 0)
                {
                    var fileName = uploadFiles[0].FileName;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "File removed failed");
            }
        }

        private void HandleException(Exception exception, string reasonPhrase)
        {
            Response.Clear();
            Response.StatusCode = 204;

            var httpResponseFeature = Response.HttpContext.Features.Get<IHttpResponseFeature>();
            if (httpResponseFeature != null)
            {
                httpResponseFeature.ReasonPhrase = reasonPhrase;
                httpResponseFeature.ReasonPhrase = exception.Message;
            }
        }
    }
}