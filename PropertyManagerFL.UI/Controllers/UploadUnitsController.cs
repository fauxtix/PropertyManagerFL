using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace PropertyManagerFL.UI.Controllers
{
    [Route("api/[controller]")]
    public class UploadUnitsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UploadUnitsController> _logger;

        public UploadUnitsController(IWebHostEnvironment hostingEnvironment, ILogger<UploadUnitsController> logger)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _logger = logger;
        }

        [HttpPost("[action]")]
        public IActionResult Save(IList<IFormFile> uploadFiles)
        {
            if (uploadFiles == null || uploadFiles.Count == 0)
            {
                return BadRequest("No files specified for upload.");
            }

            long size = 0;
            try
            {
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

                    var root = SafePath.CombineContained(
                        _hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath, "uploads", "unit_images");
                    Directory.CreateDirectory(root);
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

        [HttpPost("[action]")]
        public IActionResult Remove(IList<IFormFile> uploadFiles)
        {
            try
            {
                if (uploadFiles == null || uploadFiles.Count == 0 || uploadFiles[0] == null)
                {
                    return BadRequest("No files specified for removal.");
                }

                var root = Path.Combine(_hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath, "uploads", "unit_images");
                var filename = SafePath.CombineContained(root, SafePath.FileName(uploadFiles[0].FileName));

                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                    return Ok(new { Message = "File removed successfully." });
                }
                else
                {
                    return NotFound("File not found.");
                }
            }
            catch (Exception e)
            {
                return HandleErrorResponse("Error removing file", e);
            }
        }

        private IActionResult HandleErrorResponse(string reasonPhrase, Exception exception)
        {
            _logger.LogError(exception, "UploadUnits: {Reason}", reasonPhrase);
            Response.Clear();
            Response.StatusCode = 500; // Internal Server Error

            var httpResponseFeature = Response.HttpContext.Features.Get<IHttpResponseFeature>();

            if (httpResponseFeature != null)
            {
                httpResponseFeature.ReasonPhrase = reasonPhrase;
            }
            else
            {
                // Handle the case when IHttpResponseFeature is not available.
                // You might log this or take some other action based on your application's needs.
            }

            return Json(new { ErrorMessage = reasonPhrase });
        }
    }
}
