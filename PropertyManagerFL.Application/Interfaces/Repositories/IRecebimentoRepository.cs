using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IRecebimentoRepository
    {
        Task<List<RecebimentoVM>> GetResumedData();
        Task<decimal> TotalRecebimentos(int iTipoMovimento = 0);
        Task<decimal> TotalRecebimentos_Inquilino(int idInquilino);
        Task<decimal> TotalRecebimentosPrevisto_Inquilino(int idInquilino);
        Task<RecebimentoVM> GetRecebimento_ById(int id);
        Task<IEnumerable<RecebimentoVM>> GetAll();
        Task<int> InsertRecebimento(NovoRecebimento recebimento, bool isBatchProcessing = false);
        Task<bool> UpdateRecebimento(AlteraRecebimento recebimento);
        Task<bool> UpdateRecebimentoTemp(AlteraRecebimento recebimento);
        Task<bool> DeleteRecebimento(int id);
        Task<IEnumerable<RecebimentoVM>> GeneratePagamentoRendas(IEnumerable<ArrendamentoVM> arrendamentos, int month, int year);
        Task<decimal> GetValorUltimaRendaPaga(int id);
        Task<bool> RentalProcessingPerformed(int month, int year);
        Task<int> InsertRecebimentoTemp(NovoRecebimento entity);
        Task<RecebimentoVM> GetRecebimentoTemp_ById(int id);
        Task DeleteRecebimentosTemp();
        Task<int> LogRentProcessingPerformed(NovoProcessamentoRendas record);
        Task<ProcessamentoRendas> GetProcessamentoRendas_ById(int id);
        Task<IEnumerable<RecebimentoVM>> GetAllTemp();
        Task<decimal> GetMaxValueAllowed_ManualInput(int idInquilino);
        Task<bool> AcertaPagamentoRenda(int idRecebimento, int paymentState, decimal valorAcerto);
        Task<int> ProcessMonthlyRentPayments();
        Task<IEnumerable<ProcessamentoRendasDTO>> GetMonthlyRentsProcessed(int year);
        Task<ProcessamentoRendasDTO> GetLastPeriodProcessed();
    }
}
