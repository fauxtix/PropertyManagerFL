using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Imoveis;

namespace HouseRentalSoft.Reports.BusinessObjects
{
    public class BO_Imoveis
    {
        public int ID { get; }
        public string Descricao { get; }
        public string Numero { get; }
        public string AnoConstrucao { get; }
        public string Conservacao { get; }

        public BO_Imoveis(int Id, string sDescricao, string sNumero,
            string sAnoConstrucao, string sConservacao)
        {
            ID = Id;
            Descricao = sDescricao;
            Numero = sNumero;
            AnoConstrucao = sAnoConstrucao;
            Conservacao = sConservacao;
        }

        public class BO_Imovel
        {
            private List<BO_Imoveis> m_Imoveis;
            private readonly IImovelService _imovelSvc;

            public BO_Imovel(IImovelService imovelSvc)
            {
                string sEstadoCons;
                m_Imoveis = new List<BO_Imoveis>();
                _imovelSvc = imovelSvc;
                IEnumerable<ImovelVM> listImoveis = _imovelSvc.GetAll().GetAwaiter().GetResult();
                foreach (var item in listImoveis)
                {

                    sEstadoCons = item.EstadoConservacao;
                    m_Imoveis.Add(
                        new BO_Imoveis(item.Id, item.Descricao, item.Numero, item.AnoConstrucao, sEstadoCons)
                        );
                }
            }

            public List<BO_Imoveis> GetImoveis()
            {
                return m_Imoveis;
            }
        }
    }
}
