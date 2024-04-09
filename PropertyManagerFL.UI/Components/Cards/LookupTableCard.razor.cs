using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Components.Cards;
public partial class LookupTableCard
{
    [Parameter] public string? tableParam { get; set; }
    [Parameter] public string? cardContent { get; set; }
    [Parameter] public string? cardTitle { get; set; }
    [Parameter] public string? CardSymbol { get; set; }
    [Parameter] public string? CardType { get; set; } = "card-1";

    protected DialogEffect Effects = DialogEffect.Zoom;

    protected Modules modulo { get; set; }
    protected string? PdfFilePath { get; set; }

    string? PageTitle;
    string? TableToValidate;
    string? FkToValidate;

    protected SfSpinner? SpinnerObj;

    protected bool PdfViewerVisibility { get; set; } = false;



    protected Dictionary<string, object> htmlAttribute = new Dictionary<string, object>()
{
    {"overflow", "auto" }
};

    // not async, javascript replaced
    private void NavigateToUrlAsync(string tableName)
    {
        string url = GetUrl(tableName);
        //        await JSRuntime.InvokeAsync<object>("open", url, "_blank");
        navigationManager.NavigateTo(url);
    }

    public void CloseEditDialog()
    {
        switch (modulo)
        {
            case Modules.PdfViewer:
                PdfViewerVisibility = false;
                break;
        }
    }


    private string GetUrl(string tableName)
    {
        PageTitle = cardTitle;
        TableToValidate = "";
        FkToValidate = "";

        string SqlTable = tableName; // GetDataTableName(tableName);

        switch (tableName.ToLower())
        {
            case "estadocivil":
                TableToValidate = "Proprietarios";
                FkToValidate = "ID_EstadoCivil";
                break;
            case "estadoconservacao":
                TableToValidate = "Fracoes";
                FkToValidate = "Conservacao";
                break;
            case "formapagamento":
                TableToValidate = "Recebimentos";
                FkToValidate = "ID_TipoRecebimento";

                break;
            case "situacaofracao":
                TableToValidate = "Fracoes";
                FkToValidate = "Situacao";

                break;
            case "tipocontacto":
                TableToValidate = "Contactos";
                FkToValidate = "ID_TipoContacto";

                break;
            case "tipodespesa":
                TableToValidate = "Despesas";
                FkToValidate = "ID_TipoDespesa";

                break;
            case "tipologiafracao":
                TableToValidate = "Fracoes";
                FkToValidate = "Tipologia";

                break;

            case "tipopropriedade":
                TableToValidate = "Fracoes";
                FkToValidate = "ID_TipoPropriedade";

                break;
                //case "tiporecebimento":
                //    TableToValidate = "Recebimentos";
                //    FkToValidate = "ID_TipoRecebimento";

                //    break;
        }

        string url = $"/admin/adminpanel?" + $"DataTable={SqlTable}&PageTitle={PageTitle}&" +
                     $"TableToValidate={TableToValidate}&FieldToValidate={FkToValidate}";

        return url;
    }

    void IDisposable.Dispose()
    {
        SpinnerObj?.Dispose();
    }
}