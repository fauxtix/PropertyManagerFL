using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class ContactosService : IContactosService
    {
        private readonly IContactRepository repo;
        public ContactosService(IContactRepository repoContactos)
        {
            repo = repoContactos;
        }

        // 07/11

        // INSERT INTO HistoricoAtualizacaoRendas(DataAtualizacao, ID_Atualizacao, ID_Fracao, Valor_Renda)
        //select A.Data_Inicio, 1, F.id AS Fra_ID, A.valor_renda
        //from Arrendamentos A INNER JOIN Fracoes F
        //ON A.ID_Fracao = F.Id;

        // Dados para preenchimento da datagrid (dados vêm da view vwArrendamentos na base de dados)
        //public List<Contactos> GetResumedData()
        //{
        //    return repo.GetResumedData();
        //}

        public void Delete(Contact entity)
        {
            repo.Delete(entity);
        }

        /// <summary>
        /// Devolve numro do primeiro Id de uma tabela
        /// Verifica primeiro se tabela tem dados (método TableHasData)
        /// </summary>
        /// <returns>long</returns>
        public int GetFirstId()
        {
            return repo.GetFirstId();
        }

        public long Insert(Contact entity)
        {
            return repo.Insert(entity);
        }

        public IEnumerable<Contact> Query(string where = "")
        {
            return repo.Query(where);
        }

        public Contact Query_ById(int id)
        {
            return repo.Query_ById(id);
        }

        public bool TableHasData()
        {
            return repo.TableHasData();
        }

        public void Update(Contact entity)
        {
            repo.Update(entity);
        }

        public string RegistoComErros(Contact contacto)
        {
            ContactoValidator validator = new ContactoValidator();
            ValidationResult results = validator.Validate(contacto);

            if (!results.IsValid)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var failure in results.Errors)
                {
                    sb.AppendLine(failure.ErrorMessage);
                }
                return sb.ToString();
            }

            return "";
        }
    }
}
