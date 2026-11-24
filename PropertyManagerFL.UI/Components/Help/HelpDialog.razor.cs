using Ganss.Xss;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.UI;
using System.Text.RegularExpressions;

namespace PropertyManagerFL.UI.Components.Help
{
    /// <summary>
    /// Modal dialog that renders the end-user manual for the active UI culture as
    /// sanitized HTML. The manual is read from disk on every open, so edits to the
    /// markdown files are picked up without rebuilding the application.
    /// </summary>
    public partial class HelpDialog
    {
        private const string DefaultHelpFileName = "Help.md";
        private const string DocsFolderName = "docs";

        private static readonly Regex FragmentHrefRegex =
            new("href=\"#([^\"]+)\"", RegexOptions.Compiled);

        [Inject] private IWebHostEnvironment HostEnvironment { get; set; } = default!;
        [Inject] private IStringLocalizer<App> L { get; set; } = default!;
        [Inject] private ILogger<HelpDialog> Logger { get; set; } = default!;

        /// <summary>Controls the dialog visibility. Supports <c>@bind-Visible</c>.</summary>
        [Parameter] public bool Visible { get; set; }

        /// <summary>Raised when the dialog is closed from inside (X, Esc or the close button).</summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>Optional custom title. Defaults to the localized "Help" label.</summary>
        [Parameter] public string DialogTitle { get; set; } = string.Empty;

        [Parameter] public string Width { get; set; } = "50%";

        [Parameter] public int ZIndex { get; set; } = 1000;

        private bool _isLoading;
        private bool _wasVisible;
        private string? _html;
        private string? _error;

        private string EffectiveTitle => string.IsNullOrWhiteSpace(DialogTitle)
            ? L["btnHelp"].Value
            : DialogTitle;

        protected override async Task OnParametersSetAsync()
        {
            if (Visible && !_wasVisible)
            {
                _wasVisible = true;
                await LoadHelpAsync();
            }
            else if (!Visible)
            {
                _wasVisible = false;
            }
        }

        private async Task OnDialogVisibleChanged(bool visible)
        {
            if (!visible)
            {
                _wasVisible = false;
            }

            Visible = visible;
            await VisibleChanged.InvokeAsync(visible);
        }

        private async Task CloseAsync()
        {
            _wasVisible = false;
            Visible = false;
            await VisibleChanged.InvokeAsync(false);
        }

        private async Task LoadHelpAsync()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;
            _error = null;

            try
            {
                var path = ResolveHelpPath();
                if (path is null)
                {
                    Logger.LogWarning(
                        "Help manual not found. Searched localized manual and '{DefaultFileName}' from '{ContentRoot}' and '{BaseDirectory}'.",
                        DefaultHelpFileName,
                        HostEnvironment.ContentRootPath,
                        AppContext.BaseDirectory);

                    _html = null;
                    _error = L["HelpFileNotFound"];
                    return;
                }

                var markdown = await File.ReadAllTextAsync(path);
                _html = RenderMarkdown(markdown);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load the help manual.");
                _html = null;
                _error = string.Format(L["HelpFileLoadError"].Value, ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Looks for the localized manual before <c>docs/Help.md</c> in the content
        /// root (development) and published app, walking parent folders as a fallback.
        /// </summary>
        private string? ResolveHelpPath()
        {
            var roots = new[] { HostEnvironment.ContentRootPath, AppContext.BaseDirectory };
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var fileNames = language is "en" or "fr" or "es"
                ? new[] { $"Help.{language}.md", DefaultHelpFileName }
                : new[] { DefaultHelpFileName };

            foreach (var fileName in fileNames)
            {
                foreach (var root in roots)
                {
                    if (string.IsNullOrWhiteSpace(root))
                    {
                        continue;
                    }

                    var directory = new DirectoryInfo(root);
                    for (var depth = 0; directory is not null && depth < 6; depth++)
                    {
                        var candidate = Path.Combine(directory.FullName, DocsFolderName, fileName);
                        if (File.Exists(candidate))
                        {
                            return candidate;
                        }

                        directory = directory.Parent;
                    }
                }
            }

            return null;
        }

        private static string RenderMarkdown(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            var html = Markdown.ToHtml(markdown.Replace("\r\n", "\n"), pipeline);

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("id");
            sanitizer.AllowedAttributes.Add("target");
            sanitizer.AllowedAttributes.Add("rel");
            sanitizer.AllowedAttributes.Add("title");
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowedSchemes.Add("mailto");

            return $"<article class=\"markdown-body\">{NormalizeFragmentHrefs(sanitizer.Sanitize(html))}</article>";
        }

        /// <summary>
        /// The sanitizer percent-encodes fragments (<c>#0-guia-rapido--comecar-em-5-minutos</c>)
        /// while the heading ids generated by Markdig keep the original characters
        /// (<c>0-guia-rapido--começar-em-5-minutos</c>), so the TOC links would never
        /// find their target. Restores the decoded fragments in the rendered HTML.
        /// </summary>
        private static string NormalizeFragmentHrefs(string html)
        {
            return FragmentHrefRegex.Replace(html, match =>
            {
                var fragment = match.Groups[1].Value;

                string decoded;
                try
                {
                    decoded = Uri.UnescapeDataString(fragment);
                }
                catch (Exception)
                {
                    return match.Value;
                }

                if (decoded == fragment || decoded.Contains('"'))
                {
                    return match.Value;
                }

                return $"href=\"#{decoded}\"";
            });
        }
    }
}
