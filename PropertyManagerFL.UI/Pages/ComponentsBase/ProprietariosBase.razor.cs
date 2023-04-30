using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class ProprietariosBase : ComponentBase, IDisposable
    {
        public ProprietarioVM? Owner { get; set; }
        [Inject] public IProprietarioService? OwnerService { get; set; }
        [Inject] NavigationManager? nav { get; set; }
        [Inject] public IStringLocalizer<App>? L { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }


        protected OpcoesRegisto RecordMode { get; set; }
        protected string? HeaderCaption { get; set; }
        protected bool AddEditVisibility = false;

        protected SfSpinner? SpinnerObj { get; set; }
        protected SfToast? ToastObj { get; set; }

        protected string? ToastTitle;
        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;

        protected int OwnerId;

        protected List<string> ValidationsMessages = new();
        protected bool ErrorVisibility { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "";
            ToastIcon = "";

            try
            {
                var landlordCreated = await OwnerService.TableHasData();

                if (landlordCreated)
                {
                    RecordMode = OpcoesRegisto.Gravar;
                    HeaderCaption = L["EditMsg"] + " " + L["TituloMenuProprietario"];
                    Owner = await OwnerService!.GetProprietario_ById(1);
                }
                else
                {
                    Owner = new()
                    {
                        Contacto = "",
                        DataNascimento = DateTime.Now,
                        eMail = "",
                        Identificacao = "",
                        ID_EstadoCivil = 1,
                        Morada = "",
                        Naturalidade = "",
                        NIF = "",
                        Nome = "",
                        Notas = "",
                        ValidadeCC = DateTime.Now.AddYears(2)
                    };
                    RecordMode = OpcoesRegisto.Inserir;
                    HeaderCaption = L["NewMsg"] + " " + L["TituloMenuProprietario"];
                }
                AddEditVisibility = true;

            }
            catch
            {
                throw;
            }
        }

        public async Task SaveLandlordData()
        {
            ValidationsMessages = validatorService.ValidateLandlordEntry(Owner!);

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    var updateOk = await OwnerService!.Update(Owner!.Id, Owner);
                    if (updateOk)
                    {
                        ToastTitle = L["editionMsg"] + " " + L["Record"] + " " + L["TituloMenuProprietario"];
                        ToastCss = "e-toast-success";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["FalhaGravacaoRegisto"];
                        ToastIcon = "fas fa-exclamation";
                    }

                }
                else
                {
                    OwnerId = await OwnerService!.Insert(Owner!);
                    if (OwnerId > 0)
                    {
                        ToastTitle = L["creationMsg"] + " " + L["Record"] + " " + L["TituloMenuProprietario"];
                        ToastCss = "e-toast-success";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["FalhaCriacaoRegisto"];
                        ToastIcon = "fas fa-exclamation";
                    }
                }

                AddEditVisibility = false;
                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();
                await Task.Delay(2000);

                await GotoIndex();
            }
            else
            {
                ErrorVisibility = true;
            }
        }

        protected async Task GotoIndex()
        {
            await Task.Delay(1000);
            nav?.NavigateTo("/");
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        public void CloseValidationErrorBox()
        {
            AddEditVisibility = true;
            ErrorVisibility = false;
        }

        public void Dispose()
        {
            SpinnerObj?.Dispose();
            ToastObj?.Dispose();
        }
    }
}
