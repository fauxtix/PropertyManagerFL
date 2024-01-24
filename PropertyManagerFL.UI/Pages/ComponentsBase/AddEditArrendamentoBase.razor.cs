using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFLApplication.Utilities;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;
using System.Globalization;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class AddEditArrendamentoBase : ComponentBase, IDisposable
    {
        [Inject] public IConfiguration config { get; set; }
        [Inject] public ILookupTableService? LookupTablesService { get; set; }
        [Inject] public IArrendamentoService? arrendamentosService { get; set; }
        [Inject] public IInquilinoService? inquilinosService { get; set; }
        [Inject] public IFiadorService? fiadoresService { get; set; }
        [Inject] public IImovelService? imoveisService { get; set; }
        [Inject] public IFracaoService? fracoesService { get; set; }
        [Inject] public IStringLocalizer<App>? L { get; set; }

        [Parameter] public ArrendamentoVM? SelectedLease { get; set; }
        [Parameter] public OpcoesRegisto EditMode { get; set; }
        [Parameter] public string? HeaderCaption { get; set; }

        [Parameter] public EventCallback<decimal> OnTenantBalanceChange { get; set; }

        protected int idxPaymentType;
        protected int idxProperty;
        protected int idxUnit;
        protected int idxTenant;
        protected int idxFiador;

        protected IEnumerable<LookupTableVM>? ApplicableLaws { get; set; }
        protected IEnumerable<LookupTableVM>? PaymentTypes { get; set; }
        protected IEnumerable<LookupTableVM>? InquilinosLookup { get; set; }
        protected IEnumerable<LookupTableVM>? PropertiesLookup { get; set; }
        protected IEnumerable<LookupTableVM>? UnitsLookup { get; set; }

        protected bool ShowUnitsCombo;
        protected bool ShowTenantsCombo;
        protected bool ShowNomeFiador;

        protected SfCheckBox<bool>? chbEntregaCaucao { get; set; }
        protected SfCheckBox<bool>? chbIRS { get; set; }
        protected SfCheckBox<bool>? chbVencimento { get; set; }
        protected SfComboBox<int, string?>? cboMeses { get; set; }

        protected SfNumericTextBox<decimal>? TxtSaldoInicial { get; set; }
        protected bool ShowSaldoInicial { get; set; } = false;


        protected bool blnContratoEmitido = false;
        protected bool blnPagamentosEmDia { get; set; } = true;

        protected bool InibeDocsECaucao;

        protected bool InibeCaucao;
        protected bool InibeComprovativoIRS;
        protected bool InibeVencimento;

        protected bool DocumentoVisualizado = false;
        protected decimal decSaldoCorrente = 0;
        protected int iFormaPagamento;

        protected bool ErrorVisibility { get; set; } = false;
        protected bool UserDialogVisibility { get; set; } = false;

        protected List<string> ValidationMessages = new();
        protected string? UserMessage { get; set; }
        protected string? UserDialogTitle { get; set; }

        protected bool DisableTab { get; set; } = true;
        protected bool Disabled { get; set; } = false;
        protected int SelectedTab { get; set; } = 0;
        protected OpcoesRegisto RecordMode { get; set; }

        protected string? nomePropriedade { get; set; }
        protected string? nomeFracao { get; set; }
        protected string? nomeInquilino { get; set; }
        protected string? nomeFiador { get; set; }

        // Contratos existentes
        protected decimal SaldoInquilino { get; private set; }
        protected DateTime dInicioContrato { get; set; } = DateTime.Now;
        protected int IdInquilino { get; set; }
        protected decimal Valor_Renda { get; set; }
        protected DateTime dUltimoPagamento { get; set; }
        protected int MesEscolhido;
        protected string? MesesDesdeInicio;
        protected string? TotalPrevistoUltPag;
        protected string? SaldoCorrente;
        protected string? SaldoPrevistoCorrente;
        protected string? DiferencaSaldos;
        protected int UltimoAnoPago;

        protected DateTime LeaseEnd;
        protected List<Mes> DataSource = new List<Mes>();

        // Fim - existentes


        protected class LeaseYear
        {
            public int Id { get; set; }
            public string? Year { get; set; }
        }

        protected List<LeaseYear> LeaseYearsList { get; set; } = new();
        protected int LeaseInYears { get; set; }


        protected bool DatesCanBeChanged { get; set; }

        protected Dictionary<string, object> NotesAttribute = new Dictionary<string, object>()
{
            {"rows", "3" }
    };

        protected override void OnInitialized()
        {
            for (int i = 1; i <= 20; i++)
            {
                LeaseYearsList.Add(new LeaseYear
                {
                    Id = i,
                    Year = i.ToString()
                });
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (SelectedLease is null)
                return;

            decSaldoCorrente = 0;


            LeaseEnd = SelectedLease.Data_Fim;
            var requirementsMet = await arrendamentosService!.RequirementsMet();
            if (requirementsMet == false)
            {
                ErrorVisibility = true;
                ValidationMessages = new List<string> { "Requisitos iniciais não foram preenchidos (Proprietário, Imóvel, Fração / Inquilino e Fiador)." };
                return;
            }


            PropertiesLookup = await imoveisService!.GetPropertiesAsLookupTables();

            if (EditMode == OpcoesRegisto.Gravar)
            {
                idxProperty = await imoveisService.GetCodigo_Imovel(SelectedLease.ID_Fracao);
                nomePropriedade = (await imoveisService.GetImovel_ById(idxProperty)).Descricao; ;

                UnitsLookup = await fracoesService!.GetFracoes_SemContrato(idxProperty);
                idxUnit = SelectedLease.ID_Fracao;
                nomeFracao = (await fracoesService.GetFracao_ById(idxUnit)).Descricao;

                InquilinosLookup = await inquilinosService!.GetInquilinos();

                idxTenant = SelectedLease.ID_Inquilino;
                idxFiador = SelectedLease.ID_Fiador;

                nomeInquilino = (await inquilinosService.GetInquilino_ById(idxTenant)).Nome;
                nomeFiador = (await fiadoresService.GetFiador_ById(idxFiador)).Nome;
            }

            else if (EditMode == OpcoesRegisto.Inserir)
            {
                ShowUnitsCombo = false;
                ShowTenantsCombo = false;
            }

            if (dUltimoPagamento == DateTime.MinValue)
            {
                dUltimoPagamento = DateTime.Now.AddMonths(-1);
                MesEscolhido = dUltimoPagamento.Month;
                UltimoAnoPago = dUltimoPagamento.Year;
            }

            // Modo 'Inserir', permite escolher as 3 opções, troca quando modo = 'Gravar'
            InibeDocsECaucao = false;
            InibeComprovativoIRS = false;
            InibeVencimento = false;
            InibeCaucao = false;

            blnContratoEmitido = false;
            DocumentoVisualizado = false;

            InibeDocsECaucao = Convert.ToBoolean(config.GetSection("AppSettings")["DocsECaucaoObrigatorios"]);
            if (InibeDocsECaucao && EditMode == OpcoesRegisto.Gravar)
            {
                InibeCaucao = true;
                InibeComprovativoIRS = true;
                InibeVencimento = true;
            }

            // 03/2023
            ApplicableLaws = (await arrendamentosService.GetApplicableLaws()).ToList();

            PaymentTypes = (await LookupTablesService!.GetLookupTableData("FormaPagamento")).ToList();
            idxPaymentType = SelectedLease!.FormaPagamento;

            DatesCanBeChanged = await AllowChanges();

            StateHasChanged();
        }

        // existentes


        protected void TimeInYearChangeHandler(ChangeEventArgs<int, LeaseYear> args)
        {
            LeaseInYears = args.Value;
            int yearLeaseEnd = SelectedLease.Data_Inicio.Year + LeaseInYears;
            if (yearLeaseEnd < DateTime.Now.Year)
            {
                do
                {
                    yearLeaseEnd += LeaseInYears;
                }
                while (yearLeaseEnd < DateTime.Now.Year);
            }

            var dateCalculated = new DateTime(yearLeaseEnd, SelectedLease.Data_Inicio.Month, SelectedLease.Data_Inicio.Day);
            if (dateCalculated < DateTime.Now.Date)
                dateCalculated = dateCalculated.AddYears(LeaseInYears);

            int iPrazo;
            iPrazo = Utilitarios.GetMonthsBetweenDates(dInicioContrato, dateCalculated);

            if (SelectedLease.ArrendamentoNovo)
            {
                iPrazo--;
            }

            SelectedLease.Prazo_Meses = iPrazo;
            SelectedLease.Data_Fim = dateCalculated;
            SelectedLease.Prazo = LeaseInYears;
            StateHasChanged();
        }
        protected List<string> GetMonths(DateTime StartDate)
        {
            List<string> MonthList = new List<string>();
            DateTime ThisMonth = DateTime.Now.Date;

            while (ThisMonth.Date > StartDate.Date)
            {
                MonthList.Add(ThisMonth.ToString("MMMM") + " " + ThisMonth.Year.ToString());
                ThisMonth = ThisMonth.AddMonths(-1);
            }

            return MonthList;
        }

        protected void CalculaTotais()
        {
            Valor_Renda = SelectedLease!.Valor_Renda;

            MesEscolhido = dUltimoPagamento.Month;
            if (MesEscolhido == 0 || UltimoAnoPago == 0)
                return;

            DateTime dCalcular = new DateTime(UltimoAnoPago, MesEscolhido, 1);

            int MesesUltPag = Utilitarios.GetMonthsBetweenDates(SelectedLease!.Data_Inicio, dCalcular);
            MesesDesdeInicio = MesesUltPag.ToString();
            decimal decTotRendasUltPag = Valor_Renda * MesesUltPag;
            TotalPrevistoUltPag = decTotRendasUltPag.ToString("C", new CultureInfo("pt-PT"));

            dCalcular = new DateTime(DateTime.Now.Year, DateTime.Today.Month, 1);
            int MesesCorrente = Utilitarios.GetMonthsBetweenDates(SelectedLease!.Data_Inicio, dCalcular);
            decimal decTotRendasSemAtrasos = Valor_Renda * MesesCorrente;

            int mesesAPagar = MesesCorrente - MesesUltPag;

            decimal decTotRendasAPagar = Valor_Renda * mesesAPagar;

            SaldoPrevistoCorrente = decTotRendasSemAtrasos.ToString("C", new CultureInfo("pt-PT"));

            // new 02/04/2023
            SaldoCorrente = TotalPrevistoUltPag; //ToString "0";

            if (decTotRendasSemAtrasos == decTotRendasUltPag)
            {
                DiferencaSaldos = "Pagamentos em dia";
                //SaldoCorrente = "0";
                UserDialogTitle = "Saldo corrente do Inquilino";
                UserDialogVisibility = true;
                UserMessage = "Pagamentos estão dia.";
                blnPagamentosEmDia = true;
            }
            else
            {
                DiferencaSaldos = decTotRendasAPagar.ToString("C", new CultureInfo("pt-PT"));
                DiferencaSaldos += $" ({MesesCorrente - MesesUltPag} meses * ";
                DiferencaSaldos += Valor_Renda.ToString("C", new CultureInfo("pt-PT")) + ")";

                //SaldoCorrente = ((decTotRendasSemAtrasos - decTotRendasUltPag)).ToString("C", new CultureInfo("pt-PT"));
                blnPagamentosEmDia = false;
            }

            //            dUltimoPagamento = new DateTime(UltimoAnoPago, MesEscolhido, 1);
            SelectedLease.Data_Pagamento = dUltimoPagamento;
            SelectedLease!.SaldoInicial = decTotRendasUltPag;

            OnTenantBalanceChange.InvokeAsync(decTotRendasUltPag);

            StateHasChanged();
        }

        //Fim 'Existentes'

        protected void onChangeApplicableLaw(ChangeEventArgs<int, LookupTableVM> args)
        {
            SelectedLease!.LeiVigente = args.ItemData.Descricao;
        }

        protected void onChangePaymentType(ChangeEventArgs<int, LookupTableVM> args)
        {
            idxPaymentType = args.Value;
            SelectedLease!.FormaPagamento = idxPaymentType;
        }
        protected async Task onChangeProperty(ChangeEventArgs<int, LookupTableVM> args)
        {
            idxProperty = args.Value;
            UnitsLookup = await fracoesService.GetFracoes_SemContrato(idxProperty);
            ShowUnitsCombo = true;
        }
        protected async Task onChangeUnit(ChangeEventArgs<int, LookupTableVM> args)
        {
            ShowTenantsCombo = false;

            // TODO valor vem a zero, como se tivessemos escolhido fração, o que não deveria ter acontecido
            // => verificar o porquê da chamada a este método, quando é feito o duplo click na grid (ou outra intervenção)
            idxUnit = args.Value;
            if (idxUnit == 0) return;

            if (EditMode == OpcoesRegisto.Inserir)
            {
                InquilinosLookup = await inquilinosService!.GetInquilinosDisponiveis();
            }
            else
            {
                InquilinosLookup = await inquilinosService!.GetInquilinos();
            }

            if (SelectedLease!.ArrendamentoNovo)
            {
                var leaseOkForTransaction = await fracoesService!.FracaoEstaLivre(idxUnit);
                if (!leaseOkForTransaction)
                {
                    ErrorVisibility = true;
                    ValidationMessages = new List<string> { "Fração não está disponível para arrendamento" };
                    return;
                }
            }
            else
            {
                // TODO existing lease - see if there is an active lease for this unit, before continue

                ErrorVisibility = false;
                ValidationMessages.Clear();

                // comentado código abaixo e movido para o início do método ==> só estava a ser usado em contratos existentes
                // TODO - validar 

                //if (EditMode == OpcoesRegisto.Inserir)
                //{
                //    InquilinosLookup = await inquilinosService!.GetInquilinosDisponiveis();
                //}
                //else
                //{
                //    InquilinosLookup = await inquilinosService!.GetInquilinos();
                //}

            }
            ShowTenantsCombo = true;
            ;
            SelectedLease!.ID_Fracao = idxUnit;

            SelectedLease.Valor_Renda = (await fracoesService.GetFracao_ById(idxUnit)).ValorRenda;
            StateHasChanged();
        }

        protected async Task onChangeTenant(ChangeEventArgs<int, LookupTableVM> args)
        {
            // TODO - 27/02/2023 - após recuperação do projeto, começaram os problemas - esta rotina é chamada umas vezes, outras não
            idxTenant = args.Value;
            SelectedLease!.ID_Inquilino = idxTenant;

            //            decSaldoCorrente =  CalculateInitialBalance();
            //SelectedLease.SaldoInicial = decSaldoCorrente;
            //SaldoCorrente = decSaldoCorrente.ToString("C", new CultureInfo("pt-PT"));

            SaldoCorrente = SelectedLease.SaldoInicial.ToString("C", new CultureInfo("pt-PT"));

            var fiadorInquilino = await fiadoresService.GetFiador_Inquilino(idxTenant);
            if (fiadorInquilino is not null)
            {
                nomeFiador = fiadorInquilino.Nome;
                idxFiador = fiadorInquilino.Id;
                SelectedLease.ID_Fiador = idxFiador;
            }
            else
            {
                nomeFiador = "Fiador não foi encontrado. Verifique, p.f.";
                idxFiador = 0;
            }

            ShowNomeFiador = true;
        }

        protected void DtInicioChanged(ChangedEventArgs<DateTime> args)
        {
            LeaseInYears = SelectedLease.Prazo;
            DateTime dtInicio = args.Value;
            int yearLeaseEnd = dtInicio.Year  + LeaseInYears;
            if (yearLeaseEnd < DateTime.Now.Year)
            {
                do
                {
                    yearLeaseEnd += LeaseInYears;
                }
                while (yearLeaseEnd < DateTime.Now.Year);
            }

            var dateCalculated = new DateTime(yearLeaseEnd, SelectedLease.Data_Inicio.Month, SelectedLease.Data_Inicio.Day);
            if (dateCalculated < DateTime.Now.Date)
                dateCalculated = dateCalculated.AddYears(LeaseInYears);

            SelectedLease.Data_Fim = dateCalculated;
            StateHasChanged();
        }
        protected void DtUltPagamentoChanged(ChangedEventArgs<DateTime> args)
        {
            if (args.Value.Date > DateTime.Now.Date)
            {
                UserDialogTitle = "Criar arrendamento";
                UserMessage = $"Data inválida ({args.Value.Date.ToShortDateString()} posterior à data-corrente)";
                UserDialogVisibility = true;
                args.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                return;
            }

            var minimumDateAllowed = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 2, 1);
            if (args.Value.Date < minimumDateAllowed)
            {
                UserDialogTitle = "Criar arrendamento";
                UserMessage = $"Data inválida ({minimumDateAllowed.ToShortDateString()} deverá ser data mínima)";
                UserDialogVisibility = true;
                args.Value = minimumDateAllowed;
                return;
            }

            SelectedLease!.Data_Pagamento = args.Value;
            var monthDiff = GetMonthDifference(SelectedLease.Data_Inicio, SelectedLease.Data_Pagamento);
            if (monthDiff > 1)
            {
                // tem mês em atraso?
                blnPagamentosEmDia = false;
                ShowSaldoInicial = true;
                StateHasChanged();
            }
        }

        protected void DtUlPagChanged(ChangedEventArgs<DateTime> args)
        {
            UltimoAnoPago = args.Value.Year;
            StateHasChanged();
        }

        protected void DtFimChanged(ChangedEventArgs<DateTime> args)
        {
            int iPrazo;
            DateTime dInicio = SelectedLease!.Data_Inicio;
            DateTime dFim = SelectedLease!.Data_Fim;

            iPrazo = Utilitarios.GetMonthsBetweenDates(dInicio, dFim);
            if (iPrazo > 0)
            {
                LeaseEnd = SelectedLease.Data_Fim;
                SelectedLease.Prazo_Meses = iPrazo;
            }
            else
            {
                if (EditMode == OpcoesRegisto.Gravar)
                {
                    ValidationMessages = new List<string> { $"Data-fim inválida (antes: {LeaseEnd.Date.ToShortDateString()}). Corrija, p.f.." };
                    ErrorVisibility = true;
                }
            }
            StateHasChanged();
        }
        protected void SwitchChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            var state = args;
        }

        protected void onMesPagoChanged(ChangeEventArgs<int, Mes> args)
        {
            MesEscolhido = args.Value;
            StateHasChanged();
        }


        public void CloseValidationErrorBox()
        {
            ErrorVisibility = false;
        }

        protected void CalculateExisting()
        {
            if (SelectedLease!.Data_Inicio.Date > dInicioContrato.Date)
            {
                ErrorVisibility = true;
                ValidationMessages = new List<string> { "Data-início do contrato ou data do último pagamento inválida...." };
                return;
            }
            if (SelectedLease!.Data_Fim.Date < dInicioContrato.Date)
            {
                ErrorVisibility = true;
                ValidationMessages = new List<string> { "Data-fim do contrato ou data do último pagamento inválida...." };
                return;
            }
            if (dInicioContrato.Date > DateTime.Now.Date)
            {
                ErrorVisibility = true;
                ValidationMessages = new List<string> { "Data do último pagamento inválida (posterior à data corrente)...." };
                return;
            }

            if (SelectedLease!.Valor_Renda == 0)
            {
                ErrorVisibility = true;
                ValidationMessages = new List<string> { "Informe valor da renda, p.f." };
                return;
            }
            if (UltimoAnoPago > DateTime.Today.Year)
            {
                ErrorVisibility = true;
                ValidationMessages = new List<string> { "Último ano pago inválido" };
                return;
            }

            //btnOk.Enabled = true;
            StateHasChanged();
            CalculaTotais();

        }


        private decimal CalculateInitialBalance()
        {
            Valor_Renda = SelectedLease!.Valor_Renda;

            int monthsPaid = Utilitarios.GetMonthsBetweenDates(SelectedLease.Data_Inicio, SelectedLease!.Data_Fim);
            decimal decTotRendasAPagar = Valor_Renda * monthsPaid;
            return decTotRendasAPagar;
        }

        protected async Task<bool> AllowChanges()
        {
            var leaseHasPayments = await arrendamentosService!.ChildrenExists(SelectedLease!.ID_Fracao);
            return leaseHasPayments == false;
        }

        protected void OnPagamentosEmDia(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            if (args.Checked)
            {
                TxtSaldoInicial.Readonly = true;
                ShowSaldoInicial = false;
                SelectedLease.SaldoInicial = 0;
                SelectedLease.Data_Pagamento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else
            {
                TxtSaldoInicial.Readonly = false;
                ShowSaldoInicial = true;
            }
        }

        protected int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            if (endDate.Date > startDate.Date)
            {
                return 0;
            }

            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }


        public void Dispose()
        {
            chbEntregaCaucao?.Dispose();
            chbIRS?.Dispose();
            chbVencimento?.Dispose();
            cboMeses?.Dispose();
            TxtSaldoInicial?.Dispose();
        }

        protected class Mes
        {
            public string? Descricao { get; set; }
            public int Id { get; set; }
        }


        protected List<Mes> Meses = new() {
        new Mes{ Id = 1, Descricao= "Janeiro"},
        new Mes{ Id = 2, Descricao= "Fevereiro"},
        new Mes{ Id = 3, Descricao= "Março"},
        new Mes{ Id = 4, Descricao= "Abril"},
        new Mes{ Id = 5, Descricao= "Maio"},
        new Mes{ Id = 6, Descricao= "Junho"},
        new Mes{ Id = 7, Descricao= "Julho"},
        new Mes{ Id = 8, Descricao= "Agosto"},
        new Mes{ Id = 9, Descricao= "Setembro"},
        new Mes{ Id = 10, Descricao= "Outubro"},
        new Mes{ Id = 11, Descricao= "Novembro"},
        new Mes{ Id = 12, Descricao= "Dezembro"}
        };
    }
}
