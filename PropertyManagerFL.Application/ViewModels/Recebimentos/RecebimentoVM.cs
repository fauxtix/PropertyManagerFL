using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Application.ViewModels.Recebimentos
{
	public class RecebimentoVM
	{
		public int Id { get; set; }
        public DateTime DataMovimento { get; set; }
        public bool Renda { get; set; } = true;
        public int Estado { get; set; }
        public decimal ValorPrevisto { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal ValorEmFalta { get; set; }
        public string? Notas { get; set; }
        public int ID_Propriedade { get; set; }
        public int ID_TipoRecebimento { get; set; }
        public int ID_Inquilino { get; set; }
        public bool GeradoPeloPrograma { get; set; } = false;
        public string? Imovel { get; set; }
        public string? TipoRecebimento { get; set; }
        public string?  Inquilino { get; set; }
    }
}
