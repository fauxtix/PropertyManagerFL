using System.Linq;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;

/// <summary>
/// Definição de um campo de mailmerge: código técnico (inalterável), chave do rótulo localizado e categoria.
/// </summary>
public sealed record MergeField(string Code, string LabelKey, string CategoryKey);

/// <summary>
/// Catálogo dos campos que o mailmerge preenche. Cada modelo de carta/contrato tem o seu próprio conjunto,
/// obtido dos arrays <c>aCampos</c> usados pelos wrappers de emissão de documentos.
/// </summary>
public static class MergeFieldCatalog
{
    public const string CategoryGeneral = "mf-cat-general";
    public const string CategoryLandlord = "mf-cat-landlord";
    public const string CategoryTenant = "mf-cat-tenant";
    public const string CategoryProperty = "mf-cat-property";
    public const string CategoryContract = "mf-cat-contract";
    public const string CategoryRents = "mf-cat-rents";
    public const string CategoryDates = "mf-cat-dates";

    public static readonly string[] CategoryOrder = new[]
    {
        CategoryGeneral,
        CategoryLandlord,
        CategoryTenant,
        CategoryProperty,
        CategoryContract,
        CategoryRents,
        CategoryDates
    };

    private static readonly string[] CultureSuffixes =
    {
        "_Português", "_Portugues", "_Portuguese", "_Inglês", "_Ingles", "_English",
        "_Francês", "_Frances", "_French", "_Espanhol", "_Espanol", "_Spanish",
        "_pt", "_en", "_es", "_fr"
    };

    private static MergeField F(string code, string category) => new(code, "mf-" + code, category);

    private static readonly MergeField[] OutorganteFields = Enumerable.Range(1, 3).SelectMany(i => new[]
    {
        F($"NomeOutorgante_{i}", CategoryContract),
        F($"DtNasc_O_{i}", CategoryContract),
        F($"MoradaOutorgante_{i}", CategoryContract),
        F($"NaturalidadeOutorgante_{i}", CategoryContract),
        F($"EC_Outorgante_{i}", CategoryContract),
        F($"CC_Outorgante_{i}", CategoryContract),
        F($"ValidadeCC_Outorgante_{i}", CategoryContract),
        F($"NIF_Outorgante_{i}", CategoryContract)
    }).ToArray();

    private static readonly MergeField[] ContractFields = OutorganteFields.Concat(new[]
    {
        F("Letra", CategoryProperty),
        F("Andar", CategoryProperty),
        F("Lado", CategoryProperty),
        F("MoradaImovel", CategoryProperty),
        F("Freguesia", CategoryProperty),
        F("Concelho", CategoryProperty),
        F("Artigo", CategoryProperty),
        F("LicencaHabitacao", CategoryProperty),
        F("DataLicencaHabitacao", CategoryDates),
        F("Quartos", CategoryProperty),
        F("MesesContrato", CategoryContract),
        F("DataInicioContrato", CategoryDates),
        F("DiaInicioContrato", CategoryDates),
        F("MesInicioContrato", CategoryDates),
        F("AnoInicioContrato", CategoryDates),
        F("ValorRenda", CategoryRents),
        F("ValorRendaExtenso", CategoryRents)
    }).ToArray();

    private static readonly MergeField[] RentUpdateFields = new[]
    {
        F("LocalEmissao", CategoryGeneral),
        F("DataEmissao", CategoryDates),
        F("NomeSenhorio", CategoryLandlord),
        F("MoradaSenhorio", CategoryLandlord),
        F("NomeInquilino", CategoryTenant),
        F("MoradaInquilino", CategoryTenant),
        F("MoradaFracao", CategoryProperty),
        F("Coeficiente", CategoryRents),
        F("Lei", CategoryRents),
        F("DataPublicacao", CategoryDates),
        F("MatrizPredial", CategoryProperty),
        F("ValorRenda", CategoryRents),
        F("NovoValorRenda", CategoryRents),
        F("DiaAPartirDe", CategoryDates),
        F("MesAPartirDe", CategoryDates),
        F("AnoAtualizacao", CategoryDates)
    };

    private static readonly MergeField[] ManualRentUpdateFields = new[]
    {
        F("LocalEmissao", CategoryGeneral),
        F("DataEmissao", CategoryDates),
        F("NomeSenhorio", CategoryLandlord),
        F("MoradaSenhorio", CategoryLandlord),
        F("CodigoPostalSenhorio", CategoryLandlord),
        F("NomeInquilino", CategoryTenant),
        F("MoradaInquilino", CategoryTenant),
        F("CodigoPostalInquilino", CategoryTenant),
        F("MoradaFracao", CategoryProperty),
        F("Coeficiente", CategoryRents),
        F("Lei", CategoryRents),
        F("DataPublicacao", CategoryDates),
        F("MatrizPredial", CategoryProperty),
        F("ValorRenda", CategoryRents),
        F("NovoValorRenda", CategoryRents),
        F("ExtensoNovoValor", CategoryRents),
        F("DiaAPartirDe", CategoryDates),
        F("MesAPartirDe", CategoryDates),
        F("AnoAtualizacao", CategoryDates),
        F("AnoPublicacao", CategoryDates)
    };

    private static readonly MergeField[] TerminationFields = new[]
    {
        F("LocalEmissao", CategoryGeneral),
        F("DataEmissao", CategoryDates),
        F("NomeSenhorio", CategoryLandlord),
        F("MoradaSenhorio", CategoryLandlord),
        F("NomeInquilino", CategoryTenant),
        F("MoradaInquilino", CategoryTenant),
        F("MoradaFracao", CategoryProperty),
        F("InicioContrato", CategoryDates),
        F("FimContrato", CategoryDates)
    };

    private static readonly MergeField[] LateRentFields = new[]
    {
        F("LocalEmissao", CategoryGeneral),
        F("DataEmissao", CategoryDates),
        F("NomeSenhorio", CategoryLandlord),
        F("MoradaSenhorio", CategoryLandlord),
        F("NomeInquilino", CategoryTenant),
        F("MoradaInquilino", CategoryTenant),
        F("MontanteRendasEmAtraso", CategoryRents),
        F("ValorExtenso", CategoryRents),
        F("RendasEmAtraso", CategoryRents),
        F("PrazoEmDias", CategoryRents)
    };

    private static readonly Dictionary<string, MergeField[]> TemplateFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ContratoArrendamento"] = ContractFields,
        ["CartaAtualizacaoRendas"] = RentUpdateFields,
        ["CartaAtualizacaoRendasManual"] = ManualRentUpdateFields,
        ["RevogacaoContrato"] = TerminationFields,
        ["AvisoRendasEmAtraso"] = LateRentFields
    };

    /// <summary>Todos os campos conhecidos, sem duplicados (uso em modelos carregados por upload).</summary>
    public static IReadOnlyList<MergeField> AllFields { get; } = TemplateFields.Values
        .SelectMany(fields => fields)
        .GroupBy(field => field.Code, StringComparer.OrdinalIgnoreCase)
        .Select(group => group.First())
        .ToArray();

    /// <summary>
    /// Remove a extensão e o sufixo de idioma de um nome de modelo
    /// (ex.: <c>AvisoRendasEmAtraso_Ingles.dotx</c> → <c>AvisoRendasEmAtraso</c>).
    /// </summary>
    public static string NormalizeTemplateName(string? templateFileName)
    {
        if (string.IsNullOrWhiteSpace(templateFileName))
            return string.Empty;

        string name = Path.GetFileNameWithoutExtension(templateFileName.Trim());

        foreach (string suffix in CultureSuffixes)
        {
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^suffix.Length];
                break;
            }
        }

        return name;
    }

    /// <summary>
    /// Devolve os campos válidos para o modelo seleccionado. Modelos com sufixo de idioma
    /// são normalizados para o nome base. Modelos desconhecidos recebem a lista completa,
    /// agrupada por categoria.
    /// </summary>
    public static IReadOnlyList<MergeField> GetFieldsForTemplate(string? templateFileName)
    {
        string name = NormalizeTemplateName(templateFileName);
        if (name.Length == 0)
            return AllFields;

        return TemplateFields.TryGetValue(name, out MergeField[]? fields) ? fields : AllFields;
    }
}
