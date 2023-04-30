using System.Data;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IPaymentsByMonthReporting
    {
        event Action<string, int> ProgressChanged;

        Task GenerateReport_MesAno(DateTime dCaixa);
        DataTable getValoresMensais(int doctorId, int month, int year);
        DataTable getValoresMensais_Acordo(int doctorId, int month, int year, string agreement);
    }
}