namespace PropertyManagerFL.Core.Helpers;

/// <summary>
/// Formatos usados pelos campos de merge nos modelos de carta.
/// </summary>
public static class MergeFieldFormat
{
    /// <summary>
    /// Código do campo, tal como é gravado pelos modelos .dotx existentes.
    /// </summary>
    public static string Code(string fieldName) => $"MERGEFIELD  {fieldName}  \\* MERGEFORMAT ";

    /// <summary>
    /// Texto apresentado no documento: «nomeDoCampo» (U+00AB / U+00BB).
    /// </summary>
    public static string Result(string fieldName) => $"\u00AB{fieldName}\u00BB";
}
