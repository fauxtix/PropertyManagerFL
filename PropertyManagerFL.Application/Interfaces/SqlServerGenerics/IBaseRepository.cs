using System.Data;

namespace PropertyManagerFL.Application.Interfaces.SqlServerGenerics
{
	public interface IBaseRepository<T> where T : class
	{
		long Insert(T entity);
		void Update(T entity);
		void DeleteById(int Id);

		Task<long> Insert_Async(T entity);
		Task Update_Async(T entity);
		Task DeleteById_Async(int Id);
		bool EntradaExiste_BD(string campo, string str);
		Task<bool> EntradaExiste_BD_Async(string campo, string str);

		bool RecInUse(int Id);
		bool RecInUse_ByDescription(string sDescricao);
		bool TableHasData();
		Task<bool> RecInUse_Async(int Id);
		Task<bool> RecInUse_ByDescription_Async(string sDescricao);
		Task<bool> TableHasData_Async();

		int GetFirstId();
		int GetLastId();
		Task<int> GetFirstId_Async();
		Task<int> GetLastId_Async();

		IEnumerable<T> Query(string where = "");
		Task<IEnumerable<T>> Query_Async(string where = "");
		T Query_ById(int Id);
		Task<T> Query_ById_Async(int Id);
		int GetIdByDescription(string Description);
		string GetDescriptionById(int Id);
		Task<int> GetIdByDescription_Async(string Description);
		Task<string> GetDescriptionById_Async(int Id);


		List<T> Listar();
		IEnumerable<T> ListByDescription(string Description);
		IDataReader ListByDescription_Reader(string Descricao);
		IEnumerable<T> Listar_ID(int Id);
		IDataReader Listar_ID_DR(int Id);

		Task<List<T>> Listar_Async();
		Task<IEnumerable<T>> ListByDescription_Async(string Description);
		Task<IDataReader> ListByDescription_Reader_Async(string Descricao);
		Task<IEnumerable<T>> Listar_ID_Async(int Id);
		Task<IDataReader> Listar_ID_DR_Async(int Id);
	}
}
