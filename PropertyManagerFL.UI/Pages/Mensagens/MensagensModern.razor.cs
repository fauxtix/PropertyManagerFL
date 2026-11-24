using EmailService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Email;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.Messages;
using PropertyManagerFL.UI.Shared;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.Mensagens;
public partial class MensagensModern
{
    [Inject] IMessagesService? MessagesService { get; set; }
    [Inject] IEmailSender? EmailService { get; set; }
    [Inject] UserManager<IdentityUser>? UserManager { get; set; }
    [Inject] NavigationManager? NavigationManager { get; set; }
    [Inject] public IInquilinoService? inquilinosService { get; set; }
    [Inject] public IStringLocalizer<App>? L { get; set; }
    [Inject] public IConfiguration? Config { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState>? authenticationStateTask { get; set; }

    protected IEnumerable<ComposeMessageVM>? _messages { get; set; }
    protected List<ComposeMessageVM>? _receivedMessages { get; set; }
    protected List<ComposeMessageVM>? _sentMessages { get; set; }
    protected ComposeMessageVM? _message { get; set; } = new ComposeMessageVM();
    protected System.Security.Claims.ClaimsPrincipal? CurrentUser;

    protected IEnumerable<LookupTableVM>? InquilinosLookup { get; set; }

    protected string email = "";
    protected int TenantId;
    protected string senderReceiverLabel = "";

    protected string ResultMessage = "";
    protected string TenantEmail = "";
    protected bool ShowResultMessage;
    protected bool ShowTenantEmailMessage;
    protected bool ShowTenantsCombo;
    protected bool ShowSenderReceiver;
    protected string SentReceivedBy = "0";
    protected IdentityUser? user;
    protected MessageSeverity messageSeverity { get; set; }

    protected int selectedOption;
    protected string comboPlaceHolder = "";
    protected DateTime MessageDate { get; set; }

    protected string? TenantName { get; set; }

    protected override async Task OnInitializedAsync()
    {
        senderReceiverLabel = "";
        ShowTenantsCombo = false;
        ShowTenantEmailMessage = false;
        ShowResultMessage = false;
        ShowSenderReceiver = false;
        ResultMessage = "";
        TenantId = 0;

        //SentReceivedBy = "1";
        MessageDate = DateTime.Now.Date;

        await GetTenants();
        CurrentUser = (await authenticationStateTask!).User;
        user = await UserManager.FindByNameAsync(CurrentUser?.Identity?.Name);
        email = user.Email;
        await GetMessages();
    }

    protected async Task SendMessage(int opcao)
    {
        List<string> validationMessages = new();
        ResultMessage = "";

        if (selectedOption == 0)
            validationMessages.Add(L["Não escolheu opção"]);
        else if (selectedOption < 3 && _message.TenantId == 0)
            validationMessages.Add(L["Escolha Inquilino"]);
        else if (string.IsNullOrEmpty(_message!.SubjectTitle))
            validationMessages.Add(L["Assunto em falta"]);
        else if (string.IsNullOrEmpty(_message!.MessageContent))
            validationMessages.Add(L["Mensagem em falta"]);
        else if (string.IsNullOrEmpty(_message!.SenderEmail))
            validationMessages.Add(L["Remetente em falta"]);
        else if (string.IsNullOrEmpty(_message!.DestinationEmail))
            validationMessages.Add(L["Destinatário em falta"]);

        {
            if (validationMessages.Any())
            {
                foreach (var item in validationMessages)
                {
                    ResultMessage += item;
                }
                ShowResultMessage = true;
                messageSeverity = MessageSeverity.Warning;
                return;
            }
        }

        ShowResultMessage = false;
        ResultMessage = "";

        if (_message is not null)
        {
            if (MessagesService is not null)
            {
                if (TenantId == 0) // a msg a enviar não tem inquilino como origem
                {
                    _message.MessageSentOn = DateTime.UtcNow;
                    _message.SenderEmail = email; // user email logged in
                    _message.MessageType = 2; // Sent by user logged in
                }
                var success = await MessagesService.Add(_message);
                if (success)
                {
                    if (opcao == 2)
                    {
                        if (_message.MessageType == 2 || _message.MessageType == 4)
                        {
                            var recipient = _message.DestinationEmail;
                            if (string.IsNullOrEmpty(recipient))
                            {
                                ShowResultMessage = true;
                                messageSeverity = MessageSeverity.Warning;
                                ResultMessage = L["e-mail não informado!"];
                                StateHasChanged();
                                return;
                            }
                            bool validEmail = EmailService!.IsValidEmail(recipient);
                            if (validEmail)
                            {
                                var emailMessage = new EmailService.Message(new string[]
                                    {_message.SenderEmail },
                                    _message.SubjectTitle,
                                    _message.MessageContent, TenantName!, null);

                                await EmailService.SendEmailAsync(emailMessage);
                            }
                        }
                    }


                    ResultMessage = L["Mensagem enviada com sucesso"];
                    messageSeverity = MessageSeverity.Success;
                    ShowResultMessage = true;
                }
                else
                {
                    ResultMessage = L["Erro ao criar mensagem. Verifique log, p.f.."];
                    messageSeverity = MessageSeverity.Error;
                    ShowResultMessage = true;
                }
            }
            else
            {
                ResultMessage = L["Erro no processo. Contacte Admin."];
                messageSeverity = MessageSeverity.Error;
                ShowResultMessage = true;
            }
        }
        StateHasChanged();
        _message = new();

        await GetMessages();

        StateHasChanged();
    }

    protected async Task onChangeTenant(ChangeEventArgs<int, LookupTableVM> args)
    {
        TenantId = args.Value;
        _message!.TenantId = TenantId;
        ResultMessage = "";
        ShowResultMessage = false;
        ShowTenantEmailMessage = false;
        var tenant = await inquilinosService!.GetInquilino_ById(TenantId);
        TenantEmail = tenant.eMail ?? "";
        TenantName = tenant.Nome;

        if (!string.IsNullOrEmpty(TenantEmail))
        {
            if (selectedOption == 1) // enviada ao inquilino
            {
                _message!.SenderEmail = TenantEmail;
                _message.DestinationEmail = email;
            }
            else // recebida do inquilino
            {
                _message!.SenderEmail = email;
                _message.DestinationEmail = TenantEmail;
            }

        }
        else
        {
            TenantEmail = L["E-mail não consta dos dados do inquilino..."];
            messageSeverity = MessageSeverity.Warning;
            _message!.SenderEmail = "NoEmailInformed@propertyManager.pt";
            _message.DestinationEmail = email;
            ShowTenantEmailMessage = true;
            StateHasChanged();
        }

    }

    protected void HandleMessageType(ChangeArgs<string> args)
    {
        SentReceivedBy = args.Value;

        selectedOption = int.Parse(SentReceivedBy);

        _message.MessageType = selectedOption;
        _message.TenantId = 0;

        ShowSenderReceiver = true;

        switch (selectedOption)
        {
            case 1:
                senderReceiverLabel = L["e-Mail do Destinatário"];
                comboPlaceHolder = L["TituloRemetente"];
                ShowTenantsCombo = true;
                ShowResultMessage = false;
                _message.MessageReceivedOn = DateTime.UtcNow;
                _message!.DestinationEmail = email;
                break;
            case 2: // enviada ao inquilino
                senderReceiverLabel = L["e-Mail do Destinatário"];
                comboPlaceHolder = L["TituloDestinatarioMensagem"];

                ShowTenantsCombo = true;
                ShowResultMessage = false;
                _message.MessageSentOn = DateTime.UtcNow;
                _message!.SenderEmail = email;
                _message!.DestinationEmail = "";

                break;
            case 3:
                senderReceiverLabel = L["e-Mail do Remetente"];
                _message.MessageReceivedOn = DateTime.UtcNow;
                ShowTenantsCombo = false;
                _message!.DestinationEmail = email;
                break;
            case 4:
                senderReceiverLabel = L["e-Mail do Destinatário"];
                ShowTenantsCombo = false; ;
                _message.MessageSentOn = DateTime.UtcNow;
                _message!.DestinationEmail = "";
                break;
        }

        StateHasChanged();
    }

    protected void GotoIndex()
    {
        NavigationManager.NavigateTo("/");
    }

    protected async Task GetMessages()
    {
        _messages = await MessagesService.GetAllMessages();
        _receivedMessages = _messages.Where(p => p.MessageType == 1 || p.MessageType == 3).ToList();
        _sentMessages = _messages.Where(p => p.MessageType == 2 || p.MessageType == 4).ToList();

        StateHasChanged();
    }

    protected void MessageDateChanged(ChangedEventArgs<DateTime> args)
    {

        if (_message?.MessageType == 1 || _message?.MessageType == 3)
        {
            _message.MessageReceivedOn = MessageDate;
        }
        else
        {
            _message!.MessageSentOn = MessageDate;
        }
    }

    protected async Task GetTenants()
    {
        InquilinosLookup = (await inquilinosService!.GetInquilinosAsLookup()).ToList();
    }

    public async Task CRUD_Handler(Syncfusion.Blazor.Grids.ActionEventArgs<ComposeMessageVM> args)
    {
        ShowResultMessage = false;
        ResultMessage = "";

        if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
        {
            var data = args.Data;
            var messageId = data.MessageId;

            var updateOk = await MessagesService!.Save(messageId, data);
            if (updateOk)
            {
                ResultMessage = L["Mensagem atualizada com sucesso"];
                messageSeverity = MessageSeverity.Success;
                ShowResultMessage = true;
            }
            else
            {
                ResultMessage = L["Erro na atualização da mensagem"];
                messageSeverity = MessageSeverity.Error;
                ShowResultMessage = true;
                await GetMessages();
            }
        }
        else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Cancel)
        {
            // Triggers once cancel operation completes
        }
        else if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
        {
            var data = args.Data;
            var messageId = data.MessageId;
            var deleteOk = await MessagesService!.Delete(messageId);
            if (deleteOk)
            {
                ResultMessage = L["Mensagem apagada com sucesso"];
                messageSeverity = MessageSeverity.Success;
                ShowResultMessage = true;
                await GetMessages();
            }
            else
            {
                ResultMessage = L["Erro ao apagar mensagem"];
                messageSeverity = MessageSeverity.Error;
                ShowResultMessage = true;
            }

        }

    }
}
