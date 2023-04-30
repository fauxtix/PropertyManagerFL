using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using Microsoft.Reporting.NETCore;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ReportController> _logger;
        private readonly IFracaoRepository _repoFracoes;
        private readonly IImovelRepository _repoImoveis;
        private readonly IInquilinoRepository _repoInquilinos;

        public ReportController(IWebHostEnvironment webHostEnvironment, ILogger<ReportController> logger, IFracaoRepository repoFracoes, IImovelRepository repoImoveis)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _repoFracoes = repoFracoes;
            _repoImoveis = repoImoveis;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("ListaImoveis")]
        [HttpGet]
        public async Task<string> ListaImoveis()
        {
            var rdlcFile = "rptImoveis";


            var parameters = new[]
            {
                new ReportParameter("ID", "1"),
            };

            try
            {
                var reportData = await _repoImoveis.GetAll();

                var result = ExecuteReport(rdlcFile, "dsImoveis", reportData, parameters);

                var outputFile = rdlcFile.Replace("rpt", "");
                var storedPdfFilename = StoreResultPdf(result, outputFile);
                return storedPdfFilename;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }
        }

        private byte[] ExecuteReport(string rdlcFile, string dsName, object reportList, ReportParameter[] parameters = null)
        {
            try
            {
                using var report = new LocalReport();
                string pdfDirPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "rdlc");
                string rdlcFilePath = $"{pdfDirPath}\\{rdlcFile}.rdlc";

                using (Stream rs = new FileStream(rdlcFilePath, FileMode.Open, FileAccess.Read))
                    report.LoadReportDefinition(rs);

                if (reportList is not null)
                {
                    report.DataSources.Add(new ReportDataSource(dsName, reportList));
                }

                if (parameters is not null)
                    report.SetParameters(parameters);

                var pdf = report.Render("PDF");

                return pdf;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        private string StoreResultPdf(byte[] bytes, string reportName)
        {
            var reportFilename = reportName.Replace("rpt", "");

            try
            {
                Stream stream = new MemoryStream(bytes);
                stream.Position = 0;
                var pdfFile = $@"{_webHostEnvironment.ContentRootPath}\Reports\pdfs\{reportFilename}.pdf";
                var downloadPdfFile = $"{reportFilename}.pdf";
                var streamResultFile = File(stream, "application/pdf", downloadPdfFile);
                CreatePdfOnFolder(pdfFile, streamResultFile); // store pdfs for further reading

                return pdfFile;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }
        }

        private void CreatePdfOnFolder(string filename, FileStreamResult fileResult)
        {
            using (var fileStream = System.IO.File.Create(filename))
            {
                var fileStreamResult = (FileStreamResult)fileResult;
                fileStreamResult.FileStream.Seek(0, SeekOrigin.Begin);
                fileStreamResult.FileStream.CopyTo(fileStream);
                fileStreamResult.FileStream.Seek(0, SeekOrigin.Begin); //reset position to beginning. If there's any chance the FileResult will be used by a future method, this will ensure it gets left in a usable state - Suggestion by Steven Liekens
            }
        }

    }
}
