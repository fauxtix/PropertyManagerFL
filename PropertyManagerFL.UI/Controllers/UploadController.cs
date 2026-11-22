using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Infrastructure.Security;
using System.Net.Http.Headers;

namespace PropertyManagerFL.UI.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment hostingEnvironment, ILogger<UploadController> logger)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _logger = logger;
        }

        [HttpPost("[action]")]
        public void Save(IList<IFormFile> uploadFiles)
        {
            long size = 0;
            try
            {
                var root = SafePath.CombineContained(
                    _hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath, "uploads", "xxx");
                Directory.CreateDirectory(root);

                foreach (var file in uploadFiles)
                {
                    var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    if (contentDisposition != null)
                    {
                        var fileName = contentDisposition.FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var filename = SafePath.FileName(fileName.Trim('"'));
                            var filenameToCopy = SafePath.CombineContained(root, filename);
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
                        var root = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");
                        var filePath = SafePath.CombineContained(root, SafePath.FileName(fileName));

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

        [HttpPost]
        public IActionResult SaveFile(IList<IFormFile> uploadFiles, string folder)
        {
            try
            {
                if (uploadFiles == null || uploadFiles.Count == 0)
                {
                    return BadRequest("No files specified for upload.");
                }

                if (string.IsNullOrWhiteSpace(folder))
                {
                    return BadRequest("Invalid folder specified.");
                }

                var root = SafePath.CombineContained(
                    _hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath,
                    "uploads", SafePath.FileName(folder));
                Directory.CreateDirectory(root);

                long size = 0;
                foreach (var file in uploadFiles)
                {
                    if (file == null || file.Length == 0)
                    {
                        return BadRequest("Invalid file detected.");
                    }

                    var filename = ContentDispositionHeaderValue
                            .Parse(file.ContentDisposition)
                            ?.FileName
                            ?.Trim('"');

                    if (string.IsNullOrWhiteSpace(filename))
                    {
                        return BadRequest("Invalid file name detected.");
                    }

                    var filenameToCopy = SafePath.CombineContained(root, SafePath.FileName(filename));
                    size += file.Length;

                    if (!System.IO.File.Exists(filenameToCopy))
                    {
                        using (FileStream fs = System.IO.File.Create(filenameToCopy))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }

                return Ok(new { Size = size, Message = "File uploaded successfully." });
            }
            catch (Exception e)
            {
                return HandleErrorResponse("File failed to upload", e);
            }
        }

        [HttpPost]
        public ActionResult RemoveFile([FromForm] IFormFile file, string folder)
        {
            try
            {
                if (file != null)
                {
                    var root = SafePath.CombineContained(
                        _hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath,
                        "uploads", SafePath.FileName(folder));
                    var filePath = SafePath.CombineContained(root, SafePath.FileName(file.FileName));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        return Json(new { success = true, message = "File removed successfully" });
                    }

                    return Json(new { success = false, message = "File not found" });
                }

                return Json(new { success = false, message = "No file provided" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Falha ao remover ficheiro");
                return Json(new { success = false, message = "File removal failed" });
            }
        }

        private void HandleException(Exception exception, string reasonPhrase)
        {
            _logger.LogError(exception, "Upload: {Reason}", reasonPhrase);
            Response.Clear();
            Response.StatusCode = 204;

            var httpResponseFeature = Response.HttpContext.Features.Get<IHttpResponseFeature>();
            if (httpResponseFeature != null)
            {
                httpResponseFeature.ReasonPhrase = reasonPhrase;
            }
        }

        private IActionResult HandleErrorResponse(string message, Exception exception)
        {
            _logger.LogError(exception, "Upload: {Message}", message);
            return BadRequest(message);
        }
    }
}
