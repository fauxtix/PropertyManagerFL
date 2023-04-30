using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class TipoContactoService : ITipoContactoService
    {
        private readonly ITipoContactoRepository repo;

        public TipoContactoService(ITipoContactoRepository repoTipoContacto)
        {
            repo = repoTipoContacto;
        }

        // Dados para preenchimento da datagrid (dados vêm da view vwRecebimentos na base de dados)

        public void Delete(TipoContacto entity)
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

        public long Insert(TipoContacto entity)
        {
            return repo.Insert(entity);
        }

        public IEnumerable<TipoContacto> Query(string where = "")
        {
            return repo.Query(where);
        }

        public TipoContacto Query_ById(int id)
        {
            return repo.Query_ById(id);
        }

        public bool TableHasData()
        {
            return repo.TableHasData();
        }

        public void Update(TipoContacto entity)
        {
            repo.Update(entity);
        }

        public string RegistoComErros(TipoContacto tipoContacto)
        {
            TipoContactoValidator validator = new TipoContactoValidator();
            ValidationResult results = validator.Validate(tipoContacto);

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
