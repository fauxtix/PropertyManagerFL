namespace PropertyManagerFL.Application.ViewModels.Inquilinos
{
    public class DocumentoInquilinoVM
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string? DocumentPath { get; set; }
        public string? Descricao { get; set; }
        public DateTime UploadDate { get; set; }
        public string? StorageFolder { get; set; }
        public char StorageType { get; set; }

        public string? NomeInquilino { get; set; }
    }
}
