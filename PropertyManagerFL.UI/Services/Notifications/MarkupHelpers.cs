using Microsoft.AspNetCore.Components;

namespace PropertyManagerFL.UI.Services.Notifications;

public static class MarkupHelpers
{
    /// <summary>
    /// Renders raw HTML into a view/page/component
    /// </summary>
    /// <param name="htmlString">The HTML string you want to render unescaped</param>
    /// <returns>Raw HTML</returns>
    public static MarkupString ToMarkupString(this string htmlString)
    {
        return new MarkupString(htmlString);
    }
}