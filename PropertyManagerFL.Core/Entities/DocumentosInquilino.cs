namespace PropertyManagerFL.Core.Entities
{
    public class DocumentosInquilino
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int DocumentType { get; set; }
        public string? DocumentPath { get; set; }
        public string? Descricao { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReferralDate { get; set; }
        public string? StorageFolder { get; set; }
        public char StorageType { get; set; }
        public string? TipoDocumento { get; set; } = "";
    }
}
