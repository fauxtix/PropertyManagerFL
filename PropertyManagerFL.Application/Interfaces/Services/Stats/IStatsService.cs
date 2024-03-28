using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.Stats
{
    public interface IStatsService
    {

        double Percentage(int current, int iRecords);

        // Expenses
        Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses(int year);
        Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses_ByYear(int year);
        Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpending();
        Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpendings_ByYear(int year);


        // Payments
        Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments();
        Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments_ByYear(int year);
        Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByTypeAndYear(int year);
        Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByType();
        Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments_ByPaymentMethod(int year);
    }
}