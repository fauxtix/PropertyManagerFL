using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;

public partial class MergeFieldPanel : ComponentBase
{
    [Parameter] public IReadOnlyList<MergeField> Fields { get; set; } = Array.Empty<MergeField>();
    [Parameter] public EventCallback<string> OnFieldSelected { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; }

    [Inject] public IStringLocalizer<App>? L { get; set; }

    private string searchText = string.Empty;

    protected string SearchText => searchText;

    protected IEnumerable<IGrouping<string, MergeField>> GroupedFields =>
        Fields
            .Where(mf => string.IsNullOrWhiteSpace(searchText)
                         || mf.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                         || (L?[mf.LabelKey].Value ?? string.Empty).Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .GroupBy(mf => mf.CategoryKey)
            .OrderBy(group => IndexOfCategory(group.Key))
            .ToArray();

    private static int IndexOfCategory(string categoryKey)
    {
        int index = MergeFieldCatalog.CategoryOrder.IndexOf(categoryKey);
        return index < 0 ? int.MaxValue : index;
    }

    private void OnSearchInput(ChangeEventArgs args)
    {
        searchText = args.Value?.ToString() ?? string.Empty;
    }

    private Task SelectField(string code) => OnFieldSelected.InvokeAsync(code);

    private Task ClosePanel() => OnClose.InvokeAsync();
}
