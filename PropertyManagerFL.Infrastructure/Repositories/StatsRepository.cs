using Dapper;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Infrastructure.Context;
using System.Data;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class StatsRepository : IStatsRepository
    {
        private readonly DapperContext _context;

        public StatsRepository(DapperContext context)
        {
            _context = context;
        }

        // Expenses

        /// <summary>
        /// Devolve totais de despesas por categoria
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses(int year)
        {

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ExpensesSummaryData>("usp_Despesas_Categorias_GetSum",
                    param: new { year },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments(int year)
        {

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<PaymentsSummaryData>("usp_Recebimentos_GetPaymentsByYear",
                    param: new { year },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        /// <summary>
        /// Devolve totais de despesas por mês, do parâmetro "ano"
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses_ByYear(int year)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ExpensesSummaryData>("usp_Despesas_Categorias_GetExpensesByYear", param: new { year },
                     commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        /// <summary>
        /// Devolve totais de despesas por mês, do parâmetro "year", por Tipo de Despesa
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByTypeAndYear(int year)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ExpensesSummaryDataByType>("usp_Despesas_Categorias_GetExpensesByTypeAndYear", param: new { year },
                     commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        /// <summary>
        /// Devolve totais de despesas por mês e Tipo de Despesa
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByType()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ExpensesSummaryDataByType>("usp_Despesas_Categorias_GetExpensesByType",
                     commandType: CommandType.StoredProcedure);
                return result;
            }
        }



        /// <summary>
        /// Categoria com mais gastos
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpending()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ExpensesSummaryData>("usp_Despesas_GetMostSpendCategory",
                     commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        /// <summary>
        /// Categoria com mais gastos, por Ano
        /// </summary>
        /// <param name="year"></param>
        /// <returns>Lista de ExpensesSummaryData</returns>
        public async Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpendings_ByYear(int year)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ExpensesSummaryData>("usp_Despesas_GetMostSpendCategories_ByYear",
                    new { year },
                     commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public double Percentage(int current, int iRecords)
        {
            return Math.Round(Convert.ToDouble(((double)current / iRecords) * 100), 2);
        }

        private int TotalRegistos(string sTabela, int opcao)
        {
            string sOpcao = opcao == 1 ? "Activo = 1" : opcao == 2 ? "Activo = 0" : "";
            StringBuilder sbTotal = new StringBuilder();
            sbTotal.Append("SELECT COUNT(1) ");
            sbTotal.Append($"FROM {sTabela} ");
            if (!string.IsNullOrEmpty(sOpcao))
            {
                sbTotal.Append($"WHERE {sOpcao}");
            }
            using (var connection = _context.CreateConnection())
            {
                var iRet = connection.QueryFirst<int>(sbTotal.ToString());
                return iRet;
            }
        }
    }
}
