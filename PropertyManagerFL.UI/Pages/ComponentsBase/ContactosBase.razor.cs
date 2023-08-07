using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Contactos;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    /// <summary>
    /// Base class for contacts pages
    /// </summary>
    public class ContactosBase : ComponentBase, IDisposable
    {
        /// <summary>
        /// contacts service
        /// </summary>
        [Inject] public IContactosService? contactsService { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }


        /// <summary>
        /// list of Contacts
        /// </summary>
        protected IEnumerable<ContactoVM> Contacts { get; set; }
        public ContactoVM? SelectedContact { get; set; }
        protected ContactoVM? OriginalContactData { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }
        protected int ContactId { get; set; }
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption;

        protected bool AddEditVisibility { get; set; }
        protected bool DeleteVisibility { get; set; }
        protected bool WarningVisibility { get; set; }
        protected string? WarningMessage { get; set; }
        public bool ErrorVisibility { get; set; } = false;


        public DialogEffect Effect = DialogEffect.Zoom;
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfGrid<ContactoVM>? contactsGridObj { get; set; }
        protected SfToast? ToastObj { get; set; }

        protected bool IsDirty = false;
        protected List<string>? ValidationMessages = new();

        protected string? ToastTitle;
        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;



        /// <summary>
        /// Startup Contactos
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "";
            ToastIcon = "";

            AddEditVisibility = false;
            DeleteVisibility = false;
            WarningVisibility = false;
            WarningMessage = "";
            ContactId = 0;
            IsDirty = false;

            Contacts = await GetAllContacts();
            if (!Contacts.Any())
            {
                WarningMessage = "Sem dados para mostrar";
                WarningVisibility = true;
            }

        }

        /// <summary>
        /// Get all Contacts
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ContactoVM>> GetAllContacts()
        {
            IEnumerable<ContactoVM>? ContactsList = await contactsService.GetAll();
            ContactsList.OrderByDescending(p => p.Id).ToList();
            return ContactsList;
        }


        protected async Task OnContactDoubleClickHandler(RecordDoubleClickEventArgs<ContactoVM> args)
        {
            ContactId = args.RowData.Id;
            SelectedContact = await contactsService!.GetContacto_ById(ContactId);
            OriginalContactData = await contactsService.GetContacto_ById(ContactId); // TODO should use 'Clone/MemberWise'
            AddEditVisibility = true;
            EditCaption = $"Editar dados do Contacto";
            RecordMode = OpcoesRegisto.Gravar;
        }

        /// <summary>
        /// Command handler
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task OnContactCommandClicked(CommandClickEventArgs<ContactoVM> args)
        {
            ContactId = args.RowData.Id;
            SelectedContact = await contactsService!.GetContacto_ById(ContactId);

            DeleteCaption = SelectedContact?.Nome;

            OriginalContactData = await contactsService.GetContacto_ById(ContactId); // TODO should use 'Clone/MemberWise'
            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditVisibility = true;
                EditCaption = $"Editar dados do Contacto";
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();                
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteVisibility = true;
                StateHasChanged();
            }
        }

        protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "C_Grid_pdfexport")  //Id is combination of Grid's ID and itemname
            {
                await PdfExport();
            }
        }

        protected List<PdfHeaderFooterContent> HeaderContent = new List<PdfHeaderFooterContent>
        {
            new PdfHeaderFooterContent()
            {
                Type = ContentType.Text,
                Value = "Lista de Contactos",
                Position = new PdfPosition()
                 {
                        X = 0,
                        Y = 0
                },
                Style = new PdfContentStyle()
                {
                    TextBrushColor = "#000000", FontSize = 13
                }
            },
            new PdfHeaderFooterContent()
            {
                Type = ContentType.Line,
                Points = new PdfPoints()
                {
                    X1 = 0,
                    Y1 = 14,
                    X2 = 685,
                    Y2 = 14
                },
                Style = new PdfContentStyle()
                {
                    PenColor = "#000080",
                    DashStyle = PdfDashStyle.Solid
                }
             }
        };

        protected async Task PdfExport()
        {
            PdfExportProperties ExportProperties = new PdfExportProperties();
            ExportProperties.Header = new PdfHeader()
            {
                FromTop = 0,
                Height = 60,
                Contents = HeaderContent
            };

            ExportProperties.DisableAutoFitWidth = true;

            //Below code is to customize the columns width for the pdf exported grid irrespective of the actual grid columns width

            ExportProperties.Columns = new List<GridColumn>()
            {
                new GridColumn(){ Field="Nome", HeaderText="Nome", TextAlign=TextAlign.Left, Width="250"},
                new GridColumn(){ Field="Contacto", HeaderText="Contacto", TextAlign=TextAlign.Left, Width="100"},
                new GridColumn(){ Field="TipoContacto", HeaderText=" Tipo", TextAlign=TextAlign.Left, Width="150"}
            };

            await contactsGridObj.PdfExport(ExportProperties);

        }

        /// <summary>
        /// Grava registo
        /// </summary>
        /// <returns></returns>
        public async Task SaveContactData()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationMessages = validatorService.ValidateContactsEntries(SelectedContact!);

            if (ValidationMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    ToastTitle = "Gravação de dados do Contacto";

                    var updateOk = await contactsService!.AtualizaContacto(SelectedContact!.Id, SelectedContact);
                    if (updateOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = "Operação terminou com sucesso";
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = "Erro ao atualizar dados";
                        ToastIcon = "fas fa-exclamation";
                    }

                }

                else // !editMode (Insert)
                {
                    ToastTitle = "Criação de Contacto";

                    var insertOk = await contactsService!.InsereContacto(SelectedContact!);
                    if (insertOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = "Operação terminou com sucesso";
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = "Erro ao inserir imóvel na base de dados";
                        ToastIcon = "fas fa-exclamation";
                    }

                    //IsDirty = true;
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                AddEditVisibility = false;
                Contacts = await GetAllContacts();
                //await contactsGridObj!.Refresh();
            }
            else
            {
                await Task.Delay(100);
                ErrorVisibility = true;
                await Task.Delay(100);
                await SpinnerObj!.HideAsync();
            }
        }

        private void CheckIfContactData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<ContactoVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedContact!, OriginalContactData!, out differences);
            IsDirty = differences.Any();
        }

        /// <summary>
        /// Inicializa dados do Contacto/fiador
        /// </summary>
        /// <param name="args"></param>
        public void onAddContact(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = "Novo Contacto";

            SelectedContact = new ContactoVM()
            {
                Contacto = "",
                eMail = "",
                ID_TipoContacto = 1,
                Localidade = "",
                Morada = "",
                Nome = "",
                Notas = ""
            };

            AddEditVisibility = true;
        }

        /// <summary>
        /// Fecha diálogo de criação / edição do contacto
        /// Verifica se registo foi alterado
        /// </summary>
        protected async Task CloseEditDialog()
        {
            IsDirty = false;

            var comparer = new ObjectsComparer.Comparer<ContactoVM>();
            IEnumerable<Difference> differences;
            var isEqual_P = comparer.Compare(SelectedContact!, OriginalContactData!, out differences);
            if (!isEqual_P)
            {
                IsDirty = true;
            }
            else
            {
                AddEditVisibility = false;
            }
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
            ToastTitle = "Apagar Contacto";

            try
            {
                DeleteVisibility = false;
                var resultOk = await contactsService!.ApagaContacto(SelectedContact!.Id);
                if (resultOk)
                {
                    Contacts = await GetAllContacts();
                    //await contactsGridObj!.Refresh();
                    ToastCss = "e-toast-success";
                    ToastMessage = "Operação concluída com sucesso";
                    ToastIcon = "fas fa-check";
                }
                else
                {
                    ToastCss = "e-toast-danger";
                    ToastMessage = "Erro ao remover Fração";
                    ToastIcon = "fas fa-exclamation";

                    //WarningVisibility = true;
                    //WarningMessage = $"Apagar contacto - não foi possível concluir operação...";
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

            }
            catch (Exception exc)
            {

                ToastTitle = "Error";
                ToastCss = "e-toast-danger";
                ToastMessage = "Erro ao remover Contacto";
                ToastIcon = "fas fa-exclamation";

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                //WarningVisibility = true;
                //WarningMessage = $"Não foi possível concluir operação. {exc.Message}";
            }
        }

        public void ConfirmDeleteNo()
        {
            DeleteVisibility = false;
        }


        /// <summary>
        ///  Fecha diálogo de validação
        /// </summary>
        public void CloseValidationErrorBox()
        {
            ErrorVisibility = false;
            AddEditVisibility = true;
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        private protected async Task EditContact(ContactoVM contact)
        {
            SelectedContact = contact;
            ContactId = SelectedContact.Id;
            OriginalContactData = await contactsService.GetContacto_ById(ContactId); // TODO should use 'Clone/MemberWise'


            AddEditVisibility = true;
            EditCaption = $"Editar dados do Contacto";
            RecordMode = OpcoesRegisto.Gravar;
        }

        public void Dispose()
        {
            ToastObj?.Dispose();
            SpinnerObj?.Dispose();
            contactsGridObj?.Dispose();
        }
    }
}