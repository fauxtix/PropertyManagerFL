using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

namespace PropertyManagerFL.Infrastructure.Services.CommonServices
{
    public class MailMergeService : IMailMergeService
    {
        private readonly ILogger<MailMergeService> _logger;
        public MailMergeService(ILogger<MailMergeService> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Rotina para "mailmerge" (Word)
        /// </summary>
        /// <param name="pWordDoc"></param>
        /// <param name="pMergeFields"></param>
        /// <param name="pValues"></param>
        public string GeraContrato(int iCodContrato, string pWordDoc,
            string[] pMergeFields, string[] pValues, string Cabecalho, bool Referral, bool Gravar = false)
        {
            try
            {
                string extension = Path.GetExtension(pWordDoc);
                string result = "";
                string sRestFilename = "";

                string sPathDocs = @"C:\GitProjects\HouseRentalSoft\HouseRentalSoft\Reports\Docs";
                //Common.Configurations.ApplicationConfiguration.PastaDocumentos;
                if (!sPathDocs.EndsWith(@"\"))
                    sPathDocs += @"\";

                string sDir2Save = sPathDocs;

                var dd = AppDomain.CurrentDomain.GetData("DataDirectory");

                if (!Directory.Exists(sPathDocs + @"\Contratos"))
                    Directory.CreateDirectory(sPathDocs + @"\Contratos");

                sDir2Save = sPathDocs + @"\Contratos\";


                if (Gravar)
                {
                    sRestFilename = "_" + iCodContrato.ToString() + "_" + DateTime.Now.ToString("ddMMyyyyHHmm");
                    result = sDir2Save + "Ctr_" + sRestFilename;
                }
                string sOutputPDF = result + ".pdf";
                string sOutputWord = result;

                string sSourceDoc = sPathDocs + pWordDoc;
                if (!File.Exists(sPathDocs + pWordDoc))
                {
                    _logger.LogWarning("Ficheiro " + sPathDocs + pWordDoc + " não foi encontrado.\r\n\r\nVerifique, p.f.", "Erro na abrtura de ficheiro");
                    return "";
                }

                FileStream fileStreamPath = new FileStream(sSourceDoc, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(fileStreamPath, FormatType.Dotx);
                document.MailMerge.Execute(pMergeFields, pValues);


                string sOut2 = sOutputWord.Replace(@"\\\", @"\").Replace(@"\\", @"\");
                sOut2 += ".docx";
                FileStream outFileStreamPath = new FileStream(sOut2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                document.Save(outFileStreamPath, FormatType.Docx);
                document.Close();

                GeneratePDF_FromDocx(sOutputWord, sOutputPDF);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return sOut2;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private bool GeneratePDF_FromDocx(string docWord, string docWordPDF)
        {
            try
            {
                FileStream docStream = new FileStream(docWord, FileMode.Open, FileAccess.Read);
                WordDocument wordDocument = new WordDocument(docStream, Syncfusion.DocIO.FormatType.Automatic);
                DocIORenderer render = new DocIORenderer();
                PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);


                //Set page size~(não funciona...)
                pdfDocument.PageSettings.Size = PdfPageSize.A4;

                FileStream outFileStreamPath = new FileStream(docWordPDF, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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

    }
}
