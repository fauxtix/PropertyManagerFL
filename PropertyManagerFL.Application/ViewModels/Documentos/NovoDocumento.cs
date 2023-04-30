namespace PropertyManagerFL.Application.ViewModels.Documentos
{
    public class NovoDocumento
    {
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string? URL { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public int DocumentTypeId { get; set; }
    }
}
