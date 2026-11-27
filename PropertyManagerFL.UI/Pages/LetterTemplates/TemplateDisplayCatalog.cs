using System.Text.RegularExpressions;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;

/// <summary>
/// Nomes amigáveis para os modelos de carta. Os ficheiros mantêm os nomes técnicos com sufixo de idioma
/// (ex.: <c>AvisoRendasEmAtraso_Ingles.dotx</c>); aqui sólo se traduz a apresentação.
/// </summary>
public static class TemplateDisplayCatalog
{
    public const string LanguageFormatKey = "lt-template-with-language";

    private static readonly Dictionary<string, string> TitleKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ContratoArrendamento"] = "lt-template-ContratoArrendamento",
        ["CartaAtualizacaoRendas"] = "lt-template-CartaAtualizacaoRendas",
        ["CartaAtualizacaoRendasManual"] = "lt-template-CartaAtualizacaoRendasManual",
        ["RevogacaoContrato"] = "lt-template-RevogacaoContrato",
        ["AvisoRendasEmAtraso"] = "lt-template-AvisoRendasEmAtraso"
    };

    private static readonly Dictionary<string, string> LanguageKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["_Português"] = "TituloIdiomaPortugues",
        ["_Portugues"] = "TituloIdiomaPortugues",
        ["_Portuguese"] = "TituloIdiomaPortugues",
        ["_pt"] = "TituloIdiomaPortugues",
        ["_Inglês"] = "TituloIdiomaIngles",
        ["_Ingles"] = "TituloIdiomaIngles",
        ["_English"] = "TituloIdiomaIngles",
        ["_en"] = "TituloIdiomaIngles",
        ["_Francês"] = "TituloIdiomaFrances",
        ["_Frances"] = "TituloIdiomaFrances",
        ["_French"] = "TituloIdiomaFrances",
        ["_fr"] = "TituloIdiomaFrances",
        ["_Espanhol"] = "TituloIdiomaEspanhol",
        ["_Espanol"] = "TituloIdiomaEspanhol",
        ["_Spanish"] = "TituloIdiomaEspanhol",
        ["_es"] = "TituloIdiomaEspanhol"
    };

    /// <summary>
    /// Separa o nome técnico num nome base, numa chave de título (quando conhecido) e numa chave de idioma.
    /// A remoção do sufixo é feita pela mesma normalização usada pelos campos de mailmerge.
    /// </summary>
    public static TemplateDisplayParts GetDisplayParts(string? templateFileName)
    {
        string rawName = Path.GetFileNameWithoutExtension((templateFileName ?? string.Empty).Trim());
        string baseName = MergeFieldCatalog.NormalizeTemplateName(rawName);

        string? languageKey = null;
        if (baseName.Length < rawName.Length)
        {
            string suffix = rawName[baseName.Length..];
            LanguageKeys.TryGetValue(suffix, out languageKey);
        }

        TitleKeys.TryGetValue(baseName, out string? titleKey);
        return new TemplateDisplayParts(baseName, titleKey ?? string.Empty, SplitPascalCase(baseName), languageKey);
    }

    /// <summary>
    /// Indica se a lista contém modelos de mais do que um idioma. A API normalmente devolve só os ficheiros
    /// do idioma pedido; o idioma só precisa de aparecer no nome quando essa filtragem não foi possível
    /// e a lista devolve modelos misturados.
    /// </summary>
    public static bool HasMixedLanguages(IEnumerable<string?> templateFileNames)
    {
        string[] languages = templateFileNames
            .Select(name => GetDisplayParts(name).LanguageKey ?? string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return languages.Length > 1;
    }

    /// <summary>Acrescenta o idioma ao título, usando o formato localizado quando disponível.</summary>
    public static string FormatTitle(string title, string? language, string? format)
    {
        if (string.IsNullOrWhiteSpace(language) || string.IsNullOrWhiteSpace(title))
            return title;

        string pattern = format is not null && format.Contains("{0}", StringComparison.Ordinal)
            ? format
            : "{0} ({1})";

        return string.Format(pattern, title, language);
    }

    private static string SplitPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        string spaced = Regex.Replace(value, @"(?<=[a-z0-9])(?=[A-Z])", " ").Replace('_', ' ');
        return Regex.Replace(spaced, @"\s+", " ").Trim();
    }
}

/// <summary>Chaves de recurso e texto de reserva para a apresentação de um modelo.</summary>
public sealed record TemplateDisplayParts(
    string BaseName,
    string TitleKey,
    string FallbackTitle,
    string? LanguageKey);
