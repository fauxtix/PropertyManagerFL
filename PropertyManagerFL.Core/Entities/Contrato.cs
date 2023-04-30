namespace PropertyManagerFL.Core.Entities
{
    public class Contrato : DadosOutorgante
    {
        public DadosOutorgante? Proprietario { get; set; }
        public DadosOutorgante? Inquilino { get; set; }
        public DadosOutorgante? Fiador { get; set; }

        public int IdFracao { get; set; }
        public string? LetraFracao { get; set; }
        public string? Andar { get; set; }
        public string? Lado { get; set; }
        public string? MoradaImovel { get; set; }
        public string? Numero { get; set; }
        public string? FreguesiaFracao { get; set; }
        public string? ConcelhoFracao { get; set; }
        public string? Artigo { get; set; }
        public string? MatrizPredial { get; set; }
        public string? LicencaHabitacao { get; set; }
        public DateTime DataEmissaoLicencaHabitacao { get; set; }
        public string? CamaraEmissoraLicencaHabitacao { get; set; }
        public string? CertificadoEnergetico { get; set; }
        public string? ValidadeEmissaoCertificadoEnergetico { get; set; }
        public string? EmissorCertificadoEnergetico { get; set; }
        public string? Quartos { get; set; }
        public int Prazo { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termo { get; set; }
        public decimal Valor_Renda { get; set; }
        public string? Valor_Renda_Extenso { get; set; }
        public decimal Valor_Caucao { get; set; }
        public string? Valor_Caucao_Extenso { get; set; }
        public string? NIB { get; set; }
        public bool ContratoEmitido { get; set; }
    }
}
