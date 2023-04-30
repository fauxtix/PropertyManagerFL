using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class CategoriaDespesaService : ICategoriaDespesaService
    {
        private readonly ICategoriaDespesaRepository _repoCategoriaDespesa;

        public CategoriaDespesaService(ICategoriaDespesaRepository repoCategoriaDespesa)
        {
            _repoCategoriaDespesa = repoCategoriaDespesa;
        }

        public void Delete(CategoriaDespesa entity)
        {
            _repoCategoriaDespesa.Delete(entity);
        }

        /// <summary>
        /// Devolve numro do primeiro Id de uma tabela
        /// Verifica primeiro se tabela tem dados (método TableHasData)
        /// </summary>
        /// <returns>long</returns>
        public int GetFirstId()
        {
            return _repoCategoriaDespesa.GetFirstId();
        }

        public long Insert(CategoriaDespesa entity)
        {
            return _repoCategoriaDespesa.Insert(entity);
        }

        public IEnumerable<CategoriaDespesa> Query(string where = "")
        {
            return _repoCategoriaDespesa.Query(where);
        }

        public CategoriaDespesa Query_ById(int id)
        {
            return _repoCategoriaDespesa.Query_ById(id);
        }

        public bool TableHasData()
        {
            return _repoCategoriaDespesa.TableHasData();
        }

        public void Update(CategoriaDespesa entity)
        {
            _repoCategoriaDespesa.Update(entity);
        }

        public string RegistoComErros(CategoriaDespesa categoriasDespesas)
        {
            CategoriaDespesaValidator validator = new CategoriaDespesaValidator();
            ValidationResult results = validator.Validate(categoriasDespesas);

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
