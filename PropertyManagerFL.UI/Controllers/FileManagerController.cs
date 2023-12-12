using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.PhysicalFileProvider;

namespace PropertyManagerFL.UI.Controllers
{
    [Route("api/[controller]")]
    public class FileManagerController : Controller
    {
        private readonly string _basePath;
        private readonly PhysicalFileProvider _operation;

        public FileManagerController(IWebHostEnvironment hostingEnvironment)
        {
            _basePath = hostingEnvironment.ContentRootPath ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _operation = new PhysicalFileProvider();
            _operation.RootFolder($"{_basePath}\\Data\\Files");
        }

        [HttpPost("FileOperations")]
        public IActionResult FileOperations([FromBody] FileManagerDirectoryContent? args)
        {
            if (args == null)
            {
                return BadRequest();
            }

            var result = args.Action switch
            {
                "read" => _operation.ToCamelCase(_operation.GetFiles(args.Path, args.ShowHiddenItems)),
                "delete" => _operation.ToCamelCase(_operation.Delete(args.Path, args.Names)),
                "copy" => _operation.ToCamelCase(_operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData)),
                "move" => _operation.ToCamelCase(_operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData)),
                "details" => _operation.ToCamelCase(_operation.Details(args.Path, args.Names)),
                "create" => _operation.ToCamelCase(_operation.Create(args.Path, args.Name)),
                "search" => _operation.ToCamelCase(_operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive)),
                "rename" => _operation.ToCamelCase(_operation.Rename(args.Path, args.Name, args.NewName)),
                _ => null
            };

            return result != null ? Ok(result) : BadRequest();
        }

        [HttpGet("Download")]
        public IActionResult Download(string downloadInput)
        {
            if (downloadInput is null)
            {
                return BadRequest();
            }

            if (JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput) is not { } content)
            {
                return BadRequest();
            }

            var result = _operation.Download(content.Path, content.Names);

            if (result == null)
            {
                return BadRequest("File download failed.");
            }

            return result;
        }

        [HttpPost("Upload")]
        public IActionResult Upload(string path, IList<IFormFile> uploadFiles, string action)
        {
            var uploadResponse = _operation.Upload(path, uploadFiles, action, null);

            if (uploadResponse.Error != null)
            {
                HandleErrorResponse(uploadResponse.Error);
            }

            return Content("");
        }

        [HttpGet("GetImage")]
        public IActionResult GetImage(FileManagerDirectoryContent args)
        {
            return _operation.GetImage(args.Path, null, false, null, null);
        }

        private void HandleErrorResponse(ErrorDetails error)
        {
            if (Response == null || Response.HttpContext == null || Response.HttpContext.Features == null)
            {
                return;
            }

            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.StatusCode = Convert.ToInt32(error.Code);

            var httpResponseFeature = Response.HttpContext.Features.Get<IHttpResponseFeature>();
            if (httpResponseFeature != null)
            {
                httpResponseFeature.ReasonPhrase = error.Message;
            }
        }
    }
}
