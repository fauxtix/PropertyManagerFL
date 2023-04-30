using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface IRecebimentoService
    {
        Task<List<RecebimentoVM>> GetResumedData();
        Task<decimal> TotalRecebimentos(int iTipoMovimento = 0);
        Task<decimal> TotalRecebimentos_Inquilino(int IdInquilin);
        Task<RecebimentoVM> GetRecebimento_ById(int id);
        Task<IEnumerable<RecebimentoVM>> GetAll();
        Task<IEnumerable<RecebimentoVM>> GetAllTemp();
        Task<int> InsertRecebimento(RecebimentoVM recebimento, bool isBatchProcessing = false);
        Task<int> InsertRecebimentoTemp(RecebimentoVM entity);

        Task<bool> UpdateRecebimento(int id, RecebimentoVM recebimento);
        Task<bool> DeleteRecebimento(int id);
        Task AtualizaSaldoInquilino(int IdFracao, decimal decValorRecebido);
        void CriaMovimento_CC_Inquilino(int IdPropriedade, decimal decValorRecebido, DateTime dtMovimento);
        int GetFirstId();
        int GetID_TipoRecebimento_ByDescription(string Descricao_TipoRec);
        IEnumerable<TipoRecebimento> GetLista_TipoRecebimento();
        List<Arrendamento> GetPendingContracts();
        decimal GetValorRenda(int IdFracao);
        DateTime Get_Data_Prox_Pagamento(int IdFracao);

        bool RegistoArrendamentoCriado(int IdPropriedade);

        Task<IEnumerable<RecebimentoVM>> GeneratePagamentoRendas(int month, int year);
        Task<decimal> GetValorUltimaRendaPaga(int id);
        Task<bool> RentalProcessingPerformed(int month, int year);
        Task<int> InsertRecebimentoTemp(NovoRecebimento entity);
        Task<RecebimentoVM> GetRecebimentoTemp_ById(int id);
        Task DeleteRecebimentosTemp();
        Task<bool> UpdateRecebimentoTemp(int id, RecebimentoVM recebimento);
        Task<bool> LogRentProcessingPerformed();
        Task<decimal> TotalRecebimentosPrevisto_Inquilino(int idInquilino);
        Task<decimal> GetMaxValueAllowed_ManualInput(int idInquilino);
        Task<bool> AcertaPagamentoRenda(int idRecebimento, int paymentState, decimal valorAcerto);
        Task ProcessMonthlyRentTransactions();
        Task<int> ProcessMonthlyRentPayments();
        Task<IEnumerable<ProcessamentoRendasDTO>> GetMonthlyRentsProcessed(int year);

    }
}