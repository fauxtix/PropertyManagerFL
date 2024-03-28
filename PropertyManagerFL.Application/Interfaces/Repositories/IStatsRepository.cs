using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IStatsRepository
    {
        double Percentage(int current, int iRecords);

        Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses(int year);
        Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses_ByYear(int year);
        Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpending();
        Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpendings_ByYear(int year);
        Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments(int year);
        Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByTypeAndYear(int year);
        Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByType();
    }
}
