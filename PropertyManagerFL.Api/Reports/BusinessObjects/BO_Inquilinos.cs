using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace HouseRentalSoft.Reports.BusinessObjects
{
    public class BO_Inquilinos
    {
        public int ID { get; }
        public string Nome { get; }
        public bool Titular { get; }
        public string Contacto { get; }

        public BO_Inquilinos(int Id, string sNome, string sContacto, bool bTitular)
        {
            ID = Id;
            Nome = sNome;
            Contacto = sContacto;
            Titular = bTitular;
        }

        public class BO_Inquilino
        {
            private List<BO_Inquilinos> m_Inquilinos;
            private readonly IInquilinoService _inquilinosSvc;

            public BO_Inquilino(IInquilinoService inquilinosSvc)
            {
                m_Inquilinos = new List<BO_Inquilinos>();
                _inquilinosSvc = inquilinosSvc;
                IEnumerable<InquilinoVM> lstInquilinos = _inquilinosSvc.GetAll().GetAwaiter().GetResult();
                foreach (var item in lstInquilinos)
                {
                    m_Inquilinos.Add(
                        new BO_Inquilinos(
                            item.Id, item.Nome!, item.Contacto1!, item.Titular
                            )
                        );
                }
            }

            public List<BO_Inquilinos> GetInquilinos()
            {
                return m_Inquilinos;
            }
        }
    }
}
