using System.Collections.Generic;

namespace PropertyManagerFL.Application.Interfaces.SqLiteGenerics
{
    public interface IBaseRepository<T>
    {
        long Insert(T entity);
        void Update(T entity);
        void Delete(T entity);

        // não usados, entity/classe (T), já vem configurada, o Dapper faz o mapping do Id automáticamente
        void UpdateById(int Id);
        void DeleteById(int Id);  

        bool EntradaExiste_BD(string campo, string str);
        int GetFirstId();
        int GetLastId();
        T Query_ById(int Id);
        bool RecInUse(int Id);
        bool TableHasData();

        IEnumerable<T> Query(string? where = null);
    }
}