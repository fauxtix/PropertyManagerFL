using Microsoft.AspNetCore.Components;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    /// <summary>
    /// Base class for guarantors pages
    /// </summary>
    public class FiadoresBase : ComponentBase, IDisposable
    {
        /// <summary>
        /// guarantors service
        /// </summary>
        [Inject] public IFiadorService? Fiadoreservice { get; set; }
        [Inject] public IRecebimentoService? recebimentosService { get; set; }
        [Inject] public IArrendamentoService? arrendamentoService { get; set; }
        [Inject] public IWebHostEnvironment _env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Inject] protected IValidationService validatorService { get; set; }


        /// <summary>
        /// list of guarantors
        /// </summary>
        protected IEnumerable<FiadorVM> Guarantors { get; set; }
        protected FiadorVM? SelectedGuarantor { get; set; }
        protected FiadorVM? OriginalGuarantorData { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }
        protected int guarantorId { get; set; }
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption;
        protected string? WarningMessage { get; set; }

        protected bool AddEditVisibility { get; set; }
        protected bool DeleteVisibility { get; set; }
        protected bool WarningVisibility { get; set; }
        protected bool ErrorVisibility { get; set; } = false;
        protected bool AlertVisibility { get; set; } = false;
        protected bool ShowFileVisibility { get; set; }

        protected enum guarantorFileTypes
        {
            PDF,
            WINWORD
        }


        public DialogEffect Effect = DialogEffect.Zoom;
        protected SfSpinner? SpinnerObj;

        protected bool IsDirty = false;
        protected List<string> ValidationsMessages = new();


        protected Modules modulo { get; set; }
        protected AlertMessageType? alertMessageType = AlertMessageType.Info;
        protected string? alertTitle = "";

        protected string? guarantorName { get; set; }
        protected int MaxFileSize = 5 * 1024 * 1024; // 5 MB

        protected int dataSourceFilter;

        /// <summary>
        /// Startup Fiadores
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            OriginalGuarantorData = new();
            AddEditVisibility = false;
            DeleteVisibility = false;
            DeleteCaption = "";
            ErrorVisibility = false;
            guarantorName = "";


            WarningVisibility = false;
            WarningMessage = "";
            alertTitle = "";

            AlertVisibility = false;

            guarantorId = 0;
            IsDirty = false;

            dataSourceFilter = 1;

            Guarantors = await GetAllGuarantors();
            if (!Guarantors.Any())
            {
                WarningMessage = "Sem dados para mostrar";
                WarningVisibility = true;
            }

        }

        /// <summary>
        /// Get all guarantors
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FiadorVM>> GetAllGuarantors()
        {
            IEnumerable<FiadorVM>? guarantorsList = await Fiadoreservice!.GetAll();
            guarantorsList.OrderByDescending(p => p.Id).ToList();
            return guarantorsList;
        }


        protected async Task OnGuarantorDoubleClickHandler(RecordDoubleClickEventArgs<FiadorVM> args)
        {
            guarantorId = args.RowData.Id;
            modulo = Modules.Fiadores;

            SelectedGuarantor = await Fiadoreservice!.GetFiador_ById(guarantorId);
            OriginalGuarantorData = await Fiadoreservice.GetFiador_ById(guarantorId); // TODO should use 'Clone/MemberWise'
            EditCaption = "Editar dados do Fiador";
            RecordMode = OpcoesRegisto.Gravar;
            AddEditVisibility = true;
        }


        public async Task OnGuarantorCommandClicked(CommandClickEventArgs<FiadorVM> args)
        {
            guarantorId = args.RowData.Id;
            modulo = Modules.Fiadores;
            DeleteCaption = "";

            SelectedGuarantor = await Fiadoreservice!.GetFiador_ById(guarantorId);
            OriginalGuarantorData = await Fiadoreservice.GetFiador_ById(guarantorId); // TODO should use 'Clone/MemberWise'
            DeleteCaption = SelectedGuarantor.Nome;


            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditVisibility = true;
                EditCaption = $"Editar dados do Fiador";
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteVisibility = true;
                DeleteCaption = SelectedGuarantor.Nome;
                StateHasChanged();
            }
        }


        public async Task SaveGuarantorData()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService.ValidateFiadorEntries(SelectedGuarantor!);

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    //CheckIfguarantorData_Changed(); // 08/2022
                    //if (IsDirty) // registo alterado
                    //{
                    //}

                    var updateOk = await Fiadoreservice!.AtualizaFiador(SelectedGuarantor!.Id, SelectedGuarantor);
                    if (!updateOk)
                    {
                        AlertVisibility = true;
                        alertTitle = "Atualização de dados do Fiador";
                        WarningMessage = "Erro na na operação!";
                        return;
                    }

                }
                else // !editMode (Insert)
                {
                    var insertOk = await Fiadoreservice!.InsereFiador(SelectedGuarantor!);
                    if (!insertOk)
                    {
                        AlertVisibility = true;
                        alertTitle = "Atualização de dados do Fiador";
                        WarningMessage = "Erro na na operação!";
                        return;
                    }
                }

            }
            else
            {
                //AddEditVisibility = true;
                ErrorVisibility = true;
                await Task.Delay(100);
                await SpinnerObj.HideAsync();
            }

            AddEditVisibility = false;
            Guarantors = await GetAllGuarantors();

        }

        private void CheckIfguarantorData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<FiadorVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedGuarantor!, OriginalGuarantorData!, out differences);
            IsDirty = differences.Any();
        }

        /// <summary>
        /// Inicializa dados do Fiador/fiador
        /// </summary>
        /// <param name="args"></param>
        public void onAddguarantor(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = "Novo Fiador";

            SelectedGuarantor = new FiadorVM()
            {
                Ativo = true,
                Contacto1 = "",
                Contacto2 = "",
                eMail = "",
                Identificacao = "",
                ID_EstadoCivil = 1,
                IRSAnual = 0,
                Morada = "",
                NIF = "",
                Nome = "",
                Notas = "",
                ValidadeCC = DateTime.Now,
                Vencimento = 0
            };

            AddEditVisibility = true;
        }


        protected void ContinueEdit()
        {
            IsDirty = false;
            AddEditVisibility = true;

        }

        protected void IgnoreChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            AddEditVisibility = false;
        }

        public async Task ConfirmDeleteYes()
        {
            WarningVisibility = false;
            WarningMessage = "";
            try
            {
                DeleteVisibility = false;
                var resultOk = await Fiadoreservice!.ApagaFiador(SelectedGuarantor!.Id);
                if (resultOk)
                {
                    Guarantors = await GetAllGuarantors();
                }
                else
                {
                    AddEditVisibility = false;
                    AlertVisibility = true;
                    alertTitle = "Tentativa de apagar Fiador";
                    DeleteCaption = SelectedGuarantor.Nome;
                    WarningMessage = "Fiador tem contrato de arrendamento ativo! Verifique, p.f.";
                }
            }
            catch (Exception exc)
            {
                AlertVisibility = true;
                alertTitle = "Erro ao apagar Fiador";
                WarningMessage = $"Não foi possível concluir operação. {exc.Message}";
            }
            StateHasChanged();
        }

        public void ConfirmDeleteNo()
        {
            DeleteVisibility = false;
        }

        public void CloseValidationErrorBox()
        {
            ErrorVisibility = false;
            AddEditVisibility = true;
        }

        protected void CloseEditDialog()
        {
            IsDirty = false;
            ErrorVisibility = false;

            var comparer = new ObjectsComparer.Comparer<FiadorVM>();
            var currentData = SelectedGuarantor;
            var originalData = OriginalGuarantorData;
            IEnumerable<Difference> differences_I;
            var isEqual = comparer.Compare(currentData!, originalData!, out differences_I);
            if (!isEqual)
            {
                IsDirty = true;
            }
            else
            {
                AddEditVisibility = false;
            }
        }

        public void Dispose()
        {
            SpinnerObj?.Dispose();
        }
    }
}