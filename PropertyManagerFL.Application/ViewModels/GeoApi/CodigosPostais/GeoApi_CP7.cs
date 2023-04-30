namespace PropertyManagerFL.Application.ViewModels.GeoApi.CodigosPostais
{

    public class GeoApi_CP7
    {
        public string? CP { get; set; }
        public string? CP4 { get; set; }
        public string? CP3 { get; set; }
        public string? Distrito { get; set; }
        public string? Concelho { get; set; }
        public string? Localidade { get; set; }
        public string? DesignaçãoPostal { get; set; }
        public Parte[]? partes { get; set; }
        public Ponto[]? pontos { get; set; }
        public string[]? ruas { get; set; }
        public float[]? centro { get; set; }
        public float[][]? poligono { get; set; }
        public float[]? centroide { get; set; }
        public float[]? centroDeMassa { get; set; }
    }

    public class Parte
    {
        public string? Artéria { get; set; }
        public string? Local { get; set; }
        public string? Troço { get; set; }
        public string? Porta { get; set; }
        public string? Cliente { get; set; }
    }

    public class Ponto
    {
        public string? id { get; set; }
        public string? rua { get; set; }
        public string? casa { get; set; }
        public float[]? coordenadas { get; set; }
    }


}
