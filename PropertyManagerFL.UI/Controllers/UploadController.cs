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

        [HttpPost("[action]")]
        public void Save(IList<IFormFile> uploadFiles)
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
                            var filenameToCopy = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "xxx", filename);
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

                    var filenameToCopy = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", folder, filename);
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
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", folder, fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        // Additional logic as needed
                        return Json(new { success = true, message = "File removed successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "File not found" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No file provided" });
                }
            }
            catch 
            {
                // Handle exceptions
                return Json(new { success = false, message = "File removal failed" });
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
        private IActionResult HandleErrorResponse(string message, Exception exception)
        {
            // Your error handling logic, you might want to log the exception
            // and provide a meaningful error response to the client.
            return BadRequest($"{message}. Error Details: {exception.Message}");
        }
    }
}