using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
    public class ProprietarioService : IProprietarioService
    {
        private readonly IProprietarioRepository repo;
        public ProprietarioService(IProprietarioRepository repoProprietario)
        {
            repo = repoProprietario;
        }

        public Proprietario Query_ById(int Id)
        {
            return repo.Query_ById(Id);
        }

        public int GetFistId()
        {
            return repo.GetFirstId();
        }


        public void Delete(Proprietario entity)
        {
            repo.Delete(entity);
        }

        public long Insert(Proprietario entity)
        {
            return repo.Insert(entity);
        }

        public IEnumerable<Proprietario> Query(string where = "")
        {
            return repo.Query(where);
        }

        public bool TableHasData()
        {
            return repo.TableHasData();
        }

        public void Update(Proprietario entity)
        {
            repo.Update(entity);
        }

        public string RegistoComErros(Proprietario proprietario)
        {
            ProprietarioValidator validator = new ProprietarioValidator();
            ValidationResult results = validator.Validate(proprietario);

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
