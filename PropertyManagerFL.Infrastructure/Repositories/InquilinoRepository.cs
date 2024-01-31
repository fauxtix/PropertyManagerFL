using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Infrastructure.Context;
using System.Data;
using System.Globalization;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class InquilinoRepository : IInquilinoRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<InquilinoRepository> _logger;

        /// <summary>
        /// Repositório de Inquilinos
        /// </summary>
        /// <param name="context"></param>
        public InquilinoRepository(DapperContext context, ILogger<InquilinoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsereInquilino(NovoInquilino inquilino)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Inquilinos_Insert",
                         param: inquilino, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }
        public async Task<Inquilino> AtualizaInquilino(AlteraInquilino inquilino)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var inquilinoAlterado = await connection.QuerySingleOrDefaultAsync<Inquilino>("usp_Inquilinos_Update",
                param: inquilino, commandType: CommandType.StoredProcedure);
                    return inquilinoAlterado;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
            }
        }


        public async Task AtualizaValorRenda(int unitId, decimal newRentValue)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var fracaoAlterada = await connection.ExecuteAsync("usp_Fracoes_UpdateRentValue",
                    param: new { Id = unitId, NewValue = newRentValue },
                    commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task ApagaInquilino(int id)
        {
            int idFiador = 0;
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();

                    var fiador = await connection.QueryFirstOrDefaultAsync<FiadorVM>("usp_Fiadores_Get_ByTenant",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);

                    if (fiador != null)
                    {
                        idFiador = fiador.Id;
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            // Apaga fiador do inquilino (caso tenha sido criado...)
                            if (idFiador != 0)
                            {
                                await connection.ExecuteAsync("usp_Fiadores_Delete",
                                    new { Id = idFiador }, commandType: CommandType.StoredProcedure,
                                    transaction: tran);
                            }

                            // Apaga inquilino
                            await connection.ExecuteAsync("usp_Inquilinos_Delete",
                                param: parameters, commandType: CommandType.StoredProcedure,
                                transaction: tran);


                            tran.Commit();

                        }
                        catch (Exception ex)
                        {
                            tran.Commit();
                            _logger.LogError(ex.Message, ex);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        public async Task<IEnumerable<InquilinoVM>> GetAll()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<InquilinoVM>("usp_Inquilinos_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<InquilinoVM> GetInquilino_ById(int idInquilino)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<InquilinoVM>("usp_Inquilinos_GetInquilino_Extended_ById",
                        param: new { Id = idInquilino }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (System.Data.SqlClient.SqlException ex2)
            {
                _logger.LogError(ex2.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<FiadorVM>> GeFiadortInquilino_ById(int idInquilino)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<FiadorVM>("usp_Inquilinos_GetFiador_ById",
                        param: new { Id = idInquilino }, commandType: CommandType.StoredProcedure);
                    return output;
                }
            }
            catch (System.Data.SqlClient.SqlException ex2)
            {
                _logger.LogError(ex2.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        public int GetFirstId_Inquilino()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = $"SELECT TOP 1 Id FROM Inquilinos WHERE Titular = 0 ORDER BY Id";
                int result = connection.QueryFirstOrDefault<int>(sql);
                return result;
            }
        }
        public int GetFirstIdInquilino()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = $"SELECT TOP 1 Id FROM Inquilinos WHERE Titular = 1 ORDER BY Id";
                int result = connection.QueryFirstOrDefault<int>(sql);
                return result;
            }
        }

        //// 01/12 -- não usado, para já
        //public static decimal TotalRendasPagas_Inquilino(int IdInquilino)
        //{
        //    IRecebimentoRepository repo_Rec = new RecebimentoRepository();
        //    decimal output = repo_Rec.TotalRecebimentos_Inquilino(IdInquilino);
        //    return output > 0 ? output : 0;
        //}

        public string GetNomeFracao(int IdInquilino, bool titular)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    string whereClause = titular ? $"ID_Inquilino = {IdInquilino}" : $"ID_Inquilino = {IdInquilino}";

                    int IdFracao = connection.Query<int>($"SELECT ID_Fracao FROM Arrendamentos WHERE {whereClause}")
                        .FirstOrDefault();

                    string sql = $"SELECT Descricao FROM Fracoes WHERE Id = @IdFracao";
                    string nomeFracao = connection.QueryFirstOrDefault<string>(sql, new { IdFracao });
                    return nomeFracao;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return "";
            }
        }

        public string GetUltimoMesPago_Inquilino(int ID_Inquilino)
        {
            string sOutput = "";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    DateTime dDataPagamento = connection.QueryFirstOrDefault<DateTime>
                        ($"SELECT Data_Pagamento FROM Arrendamentos WHERE ID_Inquilino = @ID_Inquilino", new { ID_Inquilino });

                    if (dDataPagamento == DateTime.MinValue)
                        sOutput = "N/D";
                    else
                    {
                        dDataPagamento = dDataPagamento.AddMonths(-1);
                        string sMesUltPagamento = dDataPagamento.ToString("MMMM", CultureInfo.CreateSpecificCulture("pt"));
                        sOutput = sMesUltPagamento + $" de {dDataPagamento.Year.ToString()}";
                    }
                }

                return sOutput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return "";
            }
        }

        public async Task<int> GetInquilinoFracao(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int id_Inquilino = await connection.QueryFirstOrDefaultAsync<int>
                        ("SELECT Id_Inquilino FROM Arrendamentos WHERE ID_Fracao = @id",
                        param: new { id });

                    return id_Inquilino;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetInquilinosDisponiveis()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var inquilinosDisponiveis = await connection.QueryAsync<LookupTableVM>("usp_Inquilinos_GetInquilinos_Disponiveis",
                         commandType: CommandType.StoredProcedure);

                    return inquilinosDisponiveis;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetInquilinosAsLookup()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var inquilinosDisponiveis = await connection.QueryAsync<LookupTableVM>("usp_Inquilinos_GetInquilinos_AsLookup",
                         commandType: CommandType.StoredProcedure);

                    return inquilinosDisponiveis;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        public async Task<string> GetNomeInquilino(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var nomeInquilino = await connection.QueryFirstOrDefaultAsync<string>("usp_Inquilinos_GetNome",
                     param: new { Id = id }, commandType: CommandType.StoredProcedure);

                return nomeInquilino;
            }

        }
        public async Task AtualizaSaldo(int id, decimal saldoCorrente)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TenantId", id);
            parameters.Add("@NovoSaldoCorrente", saldoCorrente);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("usp_Inquilinos_UpdateSaldoCorrente",
                param: parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<decimal> GetTenantRent(int Id)
        {
            using (var connection = _context.CreateConnection())
            {
                var rentValue = await connection.QuerySingleOrDefaultAsync<decimal>("usp_Inquilinos_GetValorRenda",
                new { Id }, commandType: CommandType.StoredProcedure);
                return rentValue;
            }
        }

        public async Task<bool> CanInquilinoBeDeleted(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@OkToDelete", SqlDbType.Bit, direction: ParameterDirection.Output);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<int>("usp_Inquilinos_CheckDeleteConstraint",
                    param: parameters, commandType: CommandType.StoredProcedure);

                    var areThereContracts = parameters.Get<int>("@OkToDelete");
                    return areThereContracts == 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<int> CriaDocumentoInquilino(NovoDocumentoInquilino document)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Descricao", document.Descricao);
                parameters.Add("@DocumentPath", document.DocumentPath);
                parameters.Add("@TenantId", document.TenantId);
                parameters.Add("@ReferralDate", document.ReferralDate);
                parameters.Add("@DocumentType", document.DocumentType);
                parameters.Add("@StorageType", document.StorageType);
                parameters.Add("@StorageFolder", document.StorageFolder);


                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<int>("usp_Inquilinos_InsertDocument",
                    param: parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<DocumentoInquilinoVM> GetDocumentoById(int Id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<DocumentoInquilinoVM>("[usp_Inquilinos_GetDocumentById]",
                        param: new { Id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentos()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<DocumentoInquilinoVM>("usp_Inquilinos_GetAllDocuments",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentosInquilino(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<DocumentoInquilinoVM>("usp_Inquilinos_GetTenantDocuments",
                        new { Id = id },
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<CC_Inquilino>> GetTenantPaymentsHistory(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<CC_Inquilino>("usp_Inquilinos_GetTenantPaymentsHistory",
                        new { Id = id },
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        public async Task<bool> AtualizaDocumentoInquilino(AlteraDocumentoInquilino document)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var documentoInquilinoAlterado = await connection.ExecuteAsync("usp_Inquilinos_UpdateDocument",
                        param: new { document.Id, document.Descricao }, commandType: CommandType.StoredProcedure);
                    return documentoInquilinoAlterado > 0;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            }
        }

        public async Task ApagaDocumentoInquilino(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Inquilinos_DeleteDocument",
                        new { Id = id },
                        commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetInquilinos_SemContrato()
        {

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<LookupTableVM>("usp_Inquilinos_GetInquilinos_SemContrato",
                        commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }


        public async Task<bool> TableHasData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(1) ");
            sb.Append("FROM Inquilinos");

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<int>(sb.ToString());
                return result > 0;
            }
        }

        public async Task AtualizaRendaInquilino(int unitId, DateTime leaseStart, decimal currentRentValue)
        {

            var newRentValue = await GetNewRentValue(leaseStart, currentRentValue);

            var UpdateRentHistoryParameters = new DynamicParameters();
            UpdateRentHistoryParameters.Add("@UnitId", unitId);
            UpdateRentHistoryParameters.Add("@PriorValue", currentRentValue);
            UpdateRentHistoryParameters.Add("@UpdatedValue", newRentValue);

            if (newRentValue > 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {

                        try
                        {
                            await connection.ExecuteAsync("usp_Fracoes_UpdateRentValue",
                                param: new { Id = unitId, NewValue = newRentValue },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            // criar entrada no histórico de atualização de rendas
                            await connection.ExecuteAsync("usp_Inquilinos_InsertRentUpdate",
                                param: UpdateRentHistoryParameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            tran.Commit();

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString(), ex);
                            tran.Rollback();
                        }
                    }


                }
            }
        }

        /// <summary>
        /// Verifica se Inquilino se teve atualização de rendas para o corrente ano
        /// </summary>
        /// <param name="unitId">Unit Id</param>
        /// <returns>Check if a rent update was already made for an unit</returns>
        public async Task<bool> PriorRentUpdates_ThisYear(int unitId)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<int>("usp_Inquilinos_CheckForPriorRentUpdates",
                    new { UnitId = unitId }, 
                    commandType: CommandType.StoredProcedure);
                return result > 0;
            }

        }

        public async Task AtualizaRendaInquilino_Manual(int unitId, DateTime leaseStart, decimal oldValue, decimal newValue)
        {
            var UpdateRentHistoryParameters = new DynamicParameters();
            UpdateRentHistoryParameters.Add("@UnitId", unitId);
            UpdateRentHistoryParameters.Add("@PriorValue", oldValue);
            UpdateRentHistoryParameters.Add("@UpdatedValue", newValue);

            if (newValue > 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {

                        try
                        {
                            await connection.ExecuteAsync("usp_Fracoes_UpdateRentValue",
                                param: new { Id = unitId, NewValue = newValue },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            // criar entrada no histórico de atualização de rendas
                            await connection.ExecuteAsync("usp_Inquilinos_InsertRentUpdate",
                                param: UpdateRentHistoryParameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            tran.Commit();

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString(), ex);
                            tran.Rollback();
                        }
                    }


                }
            }
        }

        /// <summary>
        /// Lista de atualizações de rendas
        /// </summary>
        /// <returns>Rent updates</returns>
        public async Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetAllRentUpdates()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<HistoricoAtualizacaoRendasVM>("usp_Inquilinos_GetAllRentUpdates",
                    commandType: CommandType.StoredProcedure);
                return result;
            }

        }

        /// <summary>
        /// Lista de atualizações de rendas por Inquilino
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Tenant rent updates</returns>
        public async Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetRentUpdates_ByTenantId(int tenantId)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<HistoricoAtualizacaoRendasVM>("usp_Inquilinos_GetRentUpdates_ByTenantId",
                   new { TenantId = tenantId},
                   commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        /// <summary>
        /// Acertos de rendas em que a transação foi apagada (não pagamento) ou alterado o seu valor (pagamento parcial
        /// </summary>
        /// <returns>registos da conta-corrente do inquilino em que houve remoção/alteração</returns>
        public async Task<IEnumerable<RentAdjustmentsVM>> GetRentAdjustments()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<RentAdjustmentsVM>("usp_Inquilinos_GetRentAdjustments",
                   commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        public async Task<(DateTime, int)> GetLeaseData_ByTenantId(int tenantId)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    dynamic result = await connection.QueryFirstOrDefaultAsync<dynamic>("[usp_Arrendamentos_GetTenantDataToUpdateRents]",
                    param: new { Id = tenantId },
                    commandType: CommandType.StoredProcedure);

                    var leaseStart = result.Data_Inicio;
                    var unitId = result.ID_Fracao;
                    return (leaseStart, unitId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex);
                return (DateTime.Now, 0);
            }
        }

        private async Task<decimal> GetNewRentValue(DateTime leaseStart, decimal valorRenda)
        {
            using (var connection = _context.CreateConnection())
            {
                var coefficient = await connection.QueryFirstOrDefaultAsync<float>("usp_Arrendamentos_Get_CurrentRentCoefficient",
                    param: new { Ano = DateTime.Now.Year },
                    commandType: CommandType.StoredProcedure);

                return valorRenda *= (decimal)coefficient;
            }
        }

        public async Task<IEnumerable<LatePaymentLettersVM>> GetLatePaymentLetters()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<LatePaymentLettersVM>("usp_Inquilinos_Stat_LatePaymentLetters",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<LatePaymentLettersVM>();
            }
        }
        public async Task<IEnumerable<LatePaymentLettersVM>> GetRentUpdateLetters()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<LatePaymentLettersVM>("usp_Inquilinos_Stat_RentUpdateLetters",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<LatePaymentLettersVM>();
            }
        }

        /// <summary>
        /// Cria documento do inquilino, após geração de carta de atualização
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantId"></param>
        /// <param name="docGerado"></param>
        /// <returns></returns>
        public async Task<bool> CriaCartaAtualizacaoInquilinoDocumentosInquilino(int tenantId, string docGerado)
        {

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Descricao", "Carta de atualização de renda");
                    parameters.Add("@DocumentPath", docGerado);
                    parameters.Add("@ReferralDate", DateTime.Now);
                    parameters.Add("@TenantId", tenantId);
                    parameters.Add("@DocumentType", 16);
                    parameters.Add("@StorageType", 'S');
                    parameters.Add("@StorageFolder", "AtualizacaoRendas");


                    await connection.ExecuteAsync("usp_Inquilinos_InsertDocument",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return false;
                }

            }
        }

    }
}
