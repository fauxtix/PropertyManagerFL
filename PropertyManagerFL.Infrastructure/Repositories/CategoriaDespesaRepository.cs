using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class CategoriaDespesaRepository : IBaseRepository<CategoriaDespesa>, ICategoriaDespesaRepository
    {
        public CategoriaDespesaRepository()
        {

        }

        public void Delete(CategoriaDespesa entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int Id)
        {
            throw new NotImplementedException();
        }

        public bool EntradaExiste_BD(string campo, string str)
        {
            throw new NotImplementedException();
        }

        public int GetFirstId()
        {
            throw new NotImplementedException();
        }

        public int GetLastId()
        {
            throw new NotImplementedException();
        }

        public long Insert(CategoriaDespesa entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CategoriaDespesa> Query(string where = "")
        {
            throw new NotImplementedException();
        }

        public CategoriaDespesa Query_ById(int Id)
        {
            throw new NotImplementedException();
        }

        public bool RecInUse(int Id)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public void Update(CategoriaDespesa entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateById(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
