using PropertyManagerFL.Application.ViewModels.MailMerge;

namespace PropertyManagerFL.Application.Interfaces.Services.Common
{
    public interface IMailMergeService
    {
        //string GeraContrato(int iCodContrato, string pWordDoc, string[] pMergeFields, string[] pValues, string Cabecalho, bool Referral, bool Gravar = false);
        Task<string> MailMergeLetter(MailMergeModel model);
    }
}