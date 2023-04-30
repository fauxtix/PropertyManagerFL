namespace PropertyManagerFL.Application.ViewModels
{
    public class AddressVM
    {
        public int ID { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Morada { get; set; }
        public string? NumeroPorta { get; set; }
        public string? Localidade { get; set; }
        public string? Freguesia { get; set; }
        public string? Concelho { get; set; }
        public int CodigoDistrito { get; set; }
        public string? Distrito { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

    }
}
