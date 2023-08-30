namespace PropertyManagerFL.Application.ViewModels.Documentos
{
    public class DocumentoVM
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string? URL { get; set; }
        public bool LocalUpload { get; set; } = true;
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public int DocumentCategoryId { get; set; }
        public string DocumentCategory { get; set; } = string.Empty;

    }
}
