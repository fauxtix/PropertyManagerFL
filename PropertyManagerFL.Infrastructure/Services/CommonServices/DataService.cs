using CommonLayer.Factories;
using Dapper;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.CommonServices
{
	public static class DataService
	{

		public static DataTable ToDataTable<T>(this IList<T> data)
		{
			PropertyDescriptorCollection propriedades =
				TypeDescriptor.GetProperties(typeof(T));
			DataTable tabela = new DataTable();
			foreach (PropertyDescriptor prop in propriedades)
				tabela.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
			foreach (T item in data)
			{
				DataRow linha = tabela.NewRow();
				foreach (PropertyDescriptor prop in propriedades)
					linha[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
				tabela.Rows.Add(linha);
			}
			return tabela;
		}

		public static bool InitializeTables()
		{
			StringBuilder sbInitialize = new StringBuilder();
			sbInitialize.Append("delete from despesa;");
			sbInitialize.Append("\ndelete from sqlite_sequence where name = 'despesa';");
			sbInitialize.Append("\ndelete from recebimento;");
			sbInitialize.Append("\ndelete from sqlite_sequence where name = 'recebimento';");
			sbInitialize.Append("\ndelete from arrendamento;");
			sbInitialize.Append("\ndelete from sqlite_sequence where name = 'arrendamento';");
			sbInitialize.Append("\ndelete from cc_inquilino;");
			sbInitialize.Append("\ndelete from sqlite_sequence where name = 'cc_inquilino';");
			sbInitialize.Append("\nupdate fracao set situacao = 2;");
			sbInitialize.Append("\nupdate inquilino set saldocorrente = 0;");
			string[] delim = { Environment.NewLine, "\n" };
			string[] lines = sbInitialize.ToString().Split(delim, StringSplitOptions.None);


			using (var connection = ConnectionManager.GetConnection())
			{
				connection.Open();
				using (var tran = connection.BeginTransaction())
				{
					try
					{
						foreach (string stmt in lines)
						{
							connection.Execute(stmt, null, tran);
						}

						tran.Commit();
						return true;
					}
					catch
					{
						tran.Rollback();
						return false;
					}
				}
			}
		}
	}
}
