namespace PropertyManagerFL.Core.Entities
{
	public class Fracao
	{
		public int Id { get; set; }
		public string? Descricao { get; set; } = "";
		public decimal ValorRenda { get; set; }
		public double AreaBrutaPrivativa { get; set; } = 0;
		public double AreaBrutaDependente { get; set; } = 0;
		public int CasasBanho { get; set; } = 0;
        public bool Varanda { get; set; } = false;
        public bool Terraco { get; set; } = false;
        public bool Garagem { get; set; } = false;
		public bool Arrecadacao { get; set; } = true;
		public bool GasCanalizado { get; set; }
		public bool CozinhaEquipada { get; set; }
		public bool LugarEstacionamento { get; set; } = false;
		public bool Fotos { get; set; } = false;
		public string? Notas { get; set; }
		public int Tipologia { get; set; } = 1;
		public string? Matriz { get; set; } = "";
        public string? LicencaHabitacao { get; set; }
        public DateTime DataEmissaoLicencaHabitacao { get; set; }
        public int ID_CertificadoEnergetico { get; set; }
        public string? Andar { get; set; } = "";
		public string? Lado { get; set; } = "";
		public string? AnoUltAvaliacao { get; set; }
		public decimal ValorUltAvaliacao { get; set; }
		public int ID_TipoPropriedade { get; set; }
		public int Id_Imovel { get; set; }
		public int Situacao { get; set; }
        public int Conservacao { get; set; }
	}
}