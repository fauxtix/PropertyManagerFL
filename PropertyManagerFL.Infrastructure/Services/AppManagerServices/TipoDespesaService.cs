using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class TipoDespesaService : ITipoDespesaService
    {
        readonly ITipoDespesaRepository repo;

        public TipoDespesaService(ITipoDespesaRepository repoTipoDespesa)
        {
            repo = repoTipoDespesa;
        }

        // Dados para preenchimento da datagrid (dados vêm da view vwRecebimentos na base de dados)

        public void Delete(TipoDespesa entity)
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

        public long Insert(TipoDespesa entity)
        {
            return repo.Insert(entity);
        }

        public IEnumerable<TipoDespesa> Query(string where = "")
        {
            return repo.Query(where);
        }

        public TipoDespesa Query_ById(int id)
        {
            return repo.Query_ById(id);
        }

        public bool TableHasData()
        {
            return repo.TableHasData();
        }

        public void Update(TipoDespesa entity)
        {
            repo.Update(entity);
        }

        public string RegistoComErros(TipoDespesa tipoDespesa)
        {
            TipoDespesaValidator validator = new TipoDespesaValidator();
            ValidationResult results = validator.Validate(tipoDespesa);

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

        public int GetID_ByDescription(string Descricao)
        {
            return repo.GetID_ByDescription(Descricao);
        }

    }
}
