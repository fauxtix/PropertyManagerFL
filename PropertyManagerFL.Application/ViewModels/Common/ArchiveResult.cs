namespace PropertyManagerFL.Application.ViewModels.Common
{
    /// <summary>
    /// Resultado da operação de arquivamento (soft delete) de inquilinos/fiadores.
    /// </summary>
    public class ArchiveResult
    {
        public bool Success { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
