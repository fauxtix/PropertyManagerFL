using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Fracoes;

namespace HouseRentalSoft.Reports.BusinessObjects
{
    public class BO_Fracoes
    {
        public int ID { get; }
        public string Descricao { get; }
        public decimal Valor_Avaliacao { get; set; }
        public string Situacao { get; }
        public string Tipo { get; }

        public BO_Fracoes(int Id, string sDescricao, decimal ValorAvaliacao, string sSituacao, string sTipo)
        {
            ID = Id;
            Descricao = sDescricao;
            Valor_Avaliacao = ValorAvaliacao;
            Situacao = sSituacao;
            Tipo = sTipo;
        }

        public class BO_Fracao
        {
            private List<BO_Fracoes> m_Fracoes;
            private readonly IFracaoService _fracoesSvc;

            public BO_Fracao(IFracaoService fracoesSvc)
            {
                string? sSitFracao;
                string? sTipoPropriedade;
                m_Fracoes = new List<BO_Fracoes>();
                _fracoesSvc = fracoesSvc;
                IEnumerable<FracaoVM> listFracoes = _fracoesSvc.GetAll().GetAwaiter().GetResult();
                foreach (var item in listFracoes)
                {
                    sSitFracao = item.SituacaoFracao;
                    sTipoPropriedade = item.TipoPropriedade; 

                    m_Fracoes.Add(
                        new BO_Fracoes(
                            item.Id, item.Descricao!, item.ValorUltAvaliacao, sSitFracao!, sTipoPropriedade!)
                        );
                }
            }

            public List<BO_Fracoes> GetFracoes()
            {
                return m_Fracoes;
            }
        }
    }
}
