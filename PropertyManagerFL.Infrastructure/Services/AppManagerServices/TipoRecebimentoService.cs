using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class TipoRecebimentoService : ITipoRecebimentoService
    {
        readonly ITipoRecebimentoRepository repo;

        public TipoRecebimentoService(ITipoRecebimentoRepository repoTipoRecebimento)
        {
            repo = repoTipoRecebimento;
        }

        // Dados para preenchimento da datagrid (dados vêm da view vwRecebimentos na base de dados)

        public void Delete(TipoRecebimento entity)
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

        public long Insert(TipoRecebimento entity)
        {
            return repo.Insert(entity);
        }

        public IEnumerable<TipoRecebimento> Query(string where = "")
        {
            return repo.Query(where);
        }

        public TipoRecebimento Query_ById(int id)
        {
            return repo.Query_ById(id);
        }

        public bool TableHasData()
        {
            return repo.TableHasData();
        }

        public void Update(TipoRecebimento entity)
        {
            repo.Update(entity);
        }

        public string RegistoComErros(TipoRecebimento tipoRecebimento)
        {
            TipoRecebimentoValidator validator = new TipoRecebimentoValidator();
            ValidationResult results = validator.Validate(tipoRecebimento);

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

        public int GetID_ByDescription(string sDescricao)
        {
            return repo.GetID_ByDescription(sDescricao);
        }
    }
}
