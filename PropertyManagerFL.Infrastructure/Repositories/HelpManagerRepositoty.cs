using CommonLayer.Factories;
using PropertyManagerFL.Infrastructure.SqLiteGenerics;
using Dapper;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Core.Entities;
using System.Text;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Factories;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class HelpManagerRepository : BaseRepository<HelpIndex>, IHelpManagerRepository
    {
        public HelpViewModel GetHelpData(int IdProjeto, string NomeForm)
        {
            using (var connection = ConnectionManager.GetConnection())
            {
                string sql = "SELECT * FROM vwHelp WHERE Id_Projeto = @IdProjeto AND NomeForm = @NomeForm";

                HelpViewModel result = connection.Query<HelpViewModel>(sql, new { IdProjeto, NomeForm }).SingleOrDefault();
                return result;
            }
        }

        public int GetIdProjeto(string NomeProjeto)
        {

            using (var connection = ConnectionManager.GetConnection())
            {
                string sql = "SELECT Id FROM HelpIndex_Parent WHERE NomeProjeto = @NomeProjeto";

                var result = connection.Query<int>(sql, new { NomeProjeto }).FirstOrDefault();
                return result;
            }
        }

        public bool HelpExists(int IdParent, string NomeForm)
        {
            using (var connection = ConnectionManager.GetConnection())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT COUNT(1) FROM HelpIndex ");
                sb.Append("WHERE Id_Parent = @IdParent AND NomeForm = @NomeForm");
                int iFormExist = connection.Query<int>(sb.ToString(), new { IdParent, NomeForm }).FirstOrDefault();
                return iFormExist > 0;
            }
        }
    }
}
