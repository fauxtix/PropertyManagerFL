using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
	public interface ILookupTableRepository
	{
		bool ActualizaDetalhes(LookupTableVM table);
		bool CheckFKInUse(int sourceFk, string fieldToCheck, string tableToCheck);
		bool CheckRegistoExist(string Descricao, string Tabela);
		bool CriaNovoRegisto(LookupTableVM tableRecord);
		bool DeleteRegisto(int iCodigo, string Tabela);
		IEnumerable<LookupTableVM> GenericGetAll(string sTabela);
		int GetCodByDescricao(string sDescr, string sTabela);
		IEnumerable<LookupTableVM> GetDataFromTabela(string sTabela, string sDescricao = "");
		string GetDescricao(int Codigo, string Tabela);
		IEnumerable<LookupTableVM> GetDescricaoByDescricao(string sDescricao, string sTabela);
		string GetDescription(int id, string tableName);
		int GetId(string Descricao, string Tabela);
		int GetLastInsertedId(string tableToCheck);
		Task<IEnumerable<LookupTableVM>> GetLookupTableData(string tableName);
		LookupTableVM GetRecordById(int id, string table);
	}
}
