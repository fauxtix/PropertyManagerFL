using System.Data;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IPaymentsReporting
    {
        DataTable Conta_IniciativaConsulta(DateTime dInicio, DateTime dFim, bool Realizadas);
        DataTable Conta_PrimeirasConsultas(DateTime dInicio, DateTime dFim, bool Realizadas);
        DataTable Conta_TipoConsulta(DateTime dInicio, DateTime dFim, bool Realizadas);
        Task GenerateReport_CaixaDia(DateTime dCaixa);
        DataTable getValoresMensais(int month, int year);
        DataTable ValoresDoDia(DateTime dMovimento);
        DataTable ValoresDoDia(int doctorId, DateTime paymentDate);

    }
}