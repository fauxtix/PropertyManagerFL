using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class TipoPropriedadeService : ITipoPropriedadeService
    {
        private readonly ITipoPropriedadeRepository repo;

        public TipoPropriedadeService(ITipoPropriedadeRepository repoTipoPropriedade)
        {
            repo = repoTipoPropriedade;
        }


        // Dados para preenchimento da datagrid (dados vêm da view vwRecebimentos na base de dados)

        public void Delete(TipoPropriedade entity)
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

        public long Insert(TipoPropriedade entity)
        {
            return repo.Insert(entity);
        }

        public IEnumerable<TipoPropriedade> Query(string where = "")
        {
            return repo.Query(where);
        }

        public TipoPropriedade Query_ById(int id)
        {
            return repo.Query_ById(id);
        }

        public bool TableHasData()
        {
            return repo.TableHasData();
        }

        public void Update(TipoPropriedade entity)
        {
            repo.Update(entity);
        }

        public string RegistoComErros(TipoPropriedade tipoPropriedade)
        {
            TipoPropriedadeValidator validator = new TipoPropriedadeValidator();
            ValidationResult results = validator.Validate(tipoPropriedade);

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
