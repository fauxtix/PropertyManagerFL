namespace PropertyManagerFL.Application.ViewModels.GeoApi.CodigosPostais
{

    public class GeoApi_CP4
    {
        public string CP4 { get; set; }
        public string[] CP3 { get; set; }
        public string Distrito { get; set; }
        public string Concelho { get; set; }
        public string[] Localidade { get; set; }
        public string[] DesignaçãoPostal { get; set; }
        public _Parte[] partes { get; set; }
        public string[] ruas { get; set; }
        public float[] centro { get; set; }
        public float[][] poligono { get; set; }
        public float[] centroide { get; set; }
        public float[] centroDeMassa { get; set; }
        public _Ponto[] pontos { get; set; }
    }

    public class _Parte
    {
        public string CP3 { get; set; }
        public string Artéria { get; set; }
        public string Localidade { get; set; }
        public string DesignaçãoPostal { get; set; }
        public string Local { get; set; }
        public string Troço { get; set; }
        public string Porta { get; set; }
        public string Cliente { get; set; }
    }

    public class _Ponto
    {
        public string rua { get; set; }
        public string casa { get; set; }
        public float[] coordenadas { get; set; }
    }

}
