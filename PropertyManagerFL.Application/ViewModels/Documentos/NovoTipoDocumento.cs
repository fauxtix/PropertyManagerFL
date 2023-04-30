namespace PropertyManagerFL.Application.ViewModels.Documentos
{
    public class NovoTipoDocumento
    {
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

    }
}
