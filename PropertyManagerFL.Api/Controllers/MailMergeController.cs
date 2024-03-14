using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.ViewModels.MailMerge;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailMergeController : ControllerBase
    {
        private readonly ILogger<MailMergeController> _logger;
        private readonly IWebHostEnvironment _environment;


        public MailMergeController(ILogger<MailMergeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("MailMergeDocument")]
        public IActionResult MailMergeDocument([FromBody] MailMergeModel model)
        {
            string Abreviatura_DocGerado = "";
            string PastaDestino = "";

            var location = GetControllerActionNames();
            try
            {
                switch (model.TipoDocumentoEmitido)
                {
                    case DocumentoEmitido.ContratoArrendamento:
                        Abreviatura_DocGerado = "Ctr";
                        PastaDestino = "Contratos";
                        break;
                    case DocumentoEmitido.AtualizacaoRendas:
                        Abreviatura_DocGerado = "ActR";
                        PastaDestino = "AtualizacaoRendas";
                        break;
                    case DocumentoEmitido.OposicaoRenovacaoContrato:
                        Abreviatura_DocGerado = "OpoRC";
                        PastaDestino = "OposicaoRenovacaoContrato";
                        break;
                    case DocumentoEmitido.RendasEmAtraso:
                        Abreviatura_DocGerado = "RenA";
                        PastaDestino = "RendasAtraso";
                        break;
                }
                string? extension = Path.GetExtension(model.WordDocument);
                string result = "";
                string sRestFilename = "";

                string sPathDocs = Path.Combine(_environment.ContentRootPath, "Reports", "Docs");
                if (!sPathDocs.EndsWith(@"\"))
                    sPathDocs += @"\";

                string sDir2Save = sPathDocs;

                if (!Directory.Exists(Path.Combine(sPathDocs, PastaDestino)))
                    Directory.CreateDirectory(Path.Combine(sPathDocs, PastaDestino));

                sDir2Save = Path.Combine(sPathDocs, PastaDestino);


                if (model.SaveFile)
                {
                    sRestFilename = "_" + model.CodContrato.ToString() + "_" + DateTime.Now.ToString("ddMMyyyyHHmm");
                    result = Path.Combine(sDir2Save, Abreviatura_DocGerado + sRestFilename);
                }
                string outputPDF = result + ".pdf";
                string outputWord = result;

                string sSourceDoc = Path.Combine(sPathDocs, model.WordDocument!);
                if (!System.IO.File.Exists(sSourceDoc))
                {
                    var notFoundMsg = "File " + sPathDocs + model.WordDocument + " not found.\r\n\r\nVerifique, p.f.";
                    _logger.LogWarning(notFoundMsg, "Error opening file");
                    return BadRequest(notFoundMsg);
                }

                FileStream fileStreamPath = new FileStream(sSourceDoc, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(fileStreamPath, FormatType.Dotx);
                document.MailMerge.Execute(model.MergeFields, model.ValuesFields);

                outputWord = outputWord.Replace(@"\\\", @"\").Replace(@"\\", @"\");
                outputWord += ".docx";
                using (FileStream outFileStreamPath = new FileStream(outputWord, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    document.Save(outFileStreamPath, FormatType.Docx);
                    document.Close();
                    document.Dispose();
                }

                GeneratePDF_FromDocx(outputWord, outputPDF);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return Ok(outputWord);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private bool GeneratePDF_FromDocx(string docWord, string docWordPDF)
        {
            try
            {
                FileStream docStream = new FileStream(docWord, FileMode.Open, FileAccess.Read);
                WordDocument wordDocument = new WordDocument(docStream, FormatType.Automatic);
                DocIORenderer render = new DocIORenderer();
                PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);


                //Set page size~(não funciona...)
                pdfDocument.PageSettings.Size = PdfPageSize.A4;

                FileStream outFileStreamPath = new FileStream(docWordPDF, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);

                pdfDocument.Save(outFileStreamPath);

                //Process.Start(docWordPDF);
                pdfDocument.Close(true);
                wordDocument.Close();

                if (wordDocument != null)
                {
                    wordDocument.Dispose();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, $"Algo de errado ocorreu ({message}). Contacte o Administrador");
        }
    }
}
