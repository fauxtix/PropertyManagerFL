using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Stats;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Core.Entities;
using static PropertyManagerFL.Application.Constants.Storage.StorageConstants;
using System.Numerics;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.UI.Pages.Despesas;
public partial class ExpensesDashboard
{
    [Inject] public IFracaoService? fracoesService { get; set; }
    [Inject] public IDespesaService? expensesService { get; set; }
    [Inject] public IStatsService? statsService { get; set; }
    [Inject] public IStringLocalizer<App> L { get; set; }
    protected ExpPieChart? PieChartRef { get; set; } = null;

    protected IEnumerable<DespesaVM>? expensesList { get; set; }
    protected IEnumerable<DespesaVM>? last5Transactions { get; set; }
    protected IEnumerable<ExpensesSummaryData>? CategoriesWithMoreSpending { get; set; }
    protected IEnumerable<ExpensesSummaryData>? CategoriesWithMoreSpendings_ByYear_Current { get; set; }
    protected IEnumerable<ExpensesSummaryData>? CategoriesWithMoreSpendings_ByYear_Prior { get; set; }

    protected IEnumerable<ExpensesSummaryDataByType>? Expenses_ByType { get; set; } = default;

    protected IEnumerable<ExpensesSummaryDataByType>? CategoriesSummaryCurrentYear { get; set; } = default;
    protected IEnumerable<ExpensesSummaryDataByType>? CategoriesSummaryPreviousYear { get; set; } = default;

    protected List<FracaoVM>? AllUnits { get; set; } = new();
    protected List<IMIResults>? IMIValues { get; set; } = new();
    protected List<IRSResults>? IRSValues { get; set; } = new();
    protected List<InsuranceResults>? InsuranceValues { get; set; } = new();

    protected decimal ExpensesTotal;
    protected decimal ExpensesThisYear;
    protected decimal ExpensesThisMonth;
    protected decimal ExpensesThisWeek;
    protected decimal ExpensesToday;
    protected string CategoryWithMoreSpendings = "";
    protected string SecondCategoryWithMoreSpendings = "";
    protected string TituloCategoriaComMaisGastos = "";
    protected string TituloSegundaCategoriaComMaisGastos = "";
    protected string? TituloCategoriaEsteAno;
    protected string? TituloCategoriaAnoAnterior;

    protected decimal totIMI;
    protected decimal totIRS;
    protected decimal totInsurance;
    protected decimal totRents;
    protected decimal totExpensesPercent;

    protected class ExpenseResults
    {
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    protected List<ExpenseResults> totExpensesList = new();

    protected override async Task OnInitializedAsync()
    {
        TituloCategoriaComMaisGastos = $"{L["TituloDespesasCategoriaComMaisGastos"]} {DateTime.Today.Year}";
        TituloSegundaCategoriaComMaisGastos = $"{L["TituloDespesasSegundaCategoriaComMaisGastos"]} {DateTime.Today.Year}";

        TituloCategoriaEsteAno = $"{L["TituloDespesasPorCategoria_Ano"]} {DateTime.Now.Year}";
        TituloCategoriaAnoAnterior = $"{L["TituloDespesasPorCategoria_Ano"]} {DateTime.Now.AddYears(-1).Year}";
        DateTime startOfWeek = DateTime.Today;
        int delta = DayOfWeek.Monday - startOfWeek.DayOfWeek;
        startOfWeek = startOfWeek.AddDays(delta);

        DateTime endOfWeek = startOfWeek.AddDays(7);

        try
        {
            AllUnits = (await fracoesService!.GetAll()).ToList();
            foreach (var unit in AllUnits)
            {
                IMIValues?.Add(
                new IMIResults
                {
                    Descricao = unit.Descricao,
                    ValPatrimonio = unit.ValorUltAvaliacao,
                    ValorPagar = Math.Round(unit.ValorUltAvaliacao * (decimal)0.0033, 2)
                });
                if (unit.Situacao == 1)
                {
                    IRSValues?.Add(
                        new IRSResults
                        {
                            Descricao = unit.Descricao,
                            ValorRenda = unit.ValorRenda * 12,
                            ValorIRS = Math.Round((unit.ValorRenda * 12) * (decimal)0.28, 2)
                        }
                        );
                }
            }

            // 04/2024
            InsuranceValues = (await fracoesService.GetUnitsInsuranceData()).ToList();

            totIMI = IMIValues!.Sum(i => i.ValorPagar);
            totIRS = IRSValues!.Sum(i => i.ValorIRS);
            totInsurance = InsuranceValues!.Sum(i => i.Premio);
            totRents = IRSValues!.Sum(i => i.ValorRenda);




            expensesList = await expensesService!.GetAll();
            last5Transactions = expensesList.OrderByDescending(l => l.DataMovimento).ToList().Take(5);
            ExpensesTotal = expensesList.Sum(e => e.Valor_Pago);
            ExpensesThisYear = expensesList.Where(e => e.DataMovimento.Year == DateTime.Today.Year).Sum(f => f.Valor_Pago);
            ExpensesThisMonth = expensesList.Where(e => e.DataMovimento.Year == DateTime.Today.Year && e.DataMovimento.Month == DateTime.Today.Month).Sum(f => f.Valor_Pago);
            ExpensesToday = expensesList.Where(e => e.DataMovimento.Date == DateTime.Today.Date).Sum(f => f.Valor_Pago);

            ExpensesThisWeek = expensesList.Where(x => (
            (x.DataMovimento >= startOfWeek && x.DataMovimento < endOfWeek) ||
            (x.DataMovimento >= startOfWeek && x.DataMovimento < endOfWeek) ||
            (x.DataMovimento >= startOfWeek && x.DataMovimento < endOfWeek)
            )).Sum(t => t.Valor_Pago);

            CategoriesWithMoreSpending = await statsService.GetExpensesCategoriesWithMoreSpending();
            // Current year's expenses (result may be 0)
            if (CategoriesWithMoreSpending.Any())
            {
                CategoryWithMoreSpendings = $"{CategoriesWithMoreSpending.ElementAt(0).Descricao} - ({CategoriesWithMoreSpending.ElementAt(0).TotalDespesas.ToString("#,###.00")})";
                if (CategoriesWithMoreSpending.Count() > 1)
                    SecondCategoryWithMoreSpendings = $"{CategoriesWithMoreSpending.ElementAt(1).Descricao} - ({CategoriesWithMoreSpending.ElementAt(1).TotalDespesas.ToString("#,###.00")})";
            }

            CategoriesWithMoreSpendings_ByYear_Current = await statsService.GetExpensesCategoriesWithMoreSpendings_ByYear(DateTime.Today.Year);
            CategoriesWithMoreSpendings_ByYear_Prior = await statsService.GetExpensesCategoriesWithMoreSpendings_ByYear(DateTime.Today.Year - 1);

            Expenses_ByType = (await statsService.GetTotalExpenses_ByType()).ToList();
            CategoriesSummaryCurrentYear = Expenses_ByType
                .Where(p => p.YearOfExpenses == DateTime.Now.Year);
            CategoriesSummaryPreviousYear = Expenses_ByType
                .Where(p => p.YearOfExpenses == DateTime.Now.Year - 1);

            var temp = (totIMI + totIRS + totInsurance + ExpensesThisYear) / totRents;
            totExpensesPercent = Math.Round(temp * 100, 2);

            totExpensesList = new() {
                new ExpenseResults { Description = "IRS", Value = totIRS },
                new ExpenseResults { Description = "IMI", Value = totIMI },
                new ExpenseResults { Description = "Seguros", Value = totInsurance },
                new ExpenseResults { Description = "Outras despesas", Value = ExpensesThisYear },
                new ExpenseResults { Description = "Total", Value = (totIRS + totIMI + totInsurance + ExpensesThisYear)},
                new ExpenseResults { Description = "% sobre Recebimentos", Value = totExpensesPercent}
                };
        }
        catch (Exception)
        {
            throw;
        }
    }
}