using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Infrastructure.Context;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using static Dapper.SqlMapper;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class RecebimentoRepository : IRecebimentoRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<RecebimentoRepository> _logger;
        private readonly ILookupTableRepository _repoLookups;
        private readonly IInquilinoRepository _repoInquilinos;
        private readonly IFracaoRepository _repoFracoes;
        private readonly IMapper _mapper;

        public RecebimentoRepository(DapperContext context,
                                     ILogger<RecebimentoRepository> logger,
                                     ILookupTableRepository repoLookups,
                                     IMapper mapper,
                                     IInquilinoRepository repoInquilinos,
                                     IFracaoRepository repoFracoes)
        {
            _context = context;
            _logger = logger;
            _repoLookups = repoLookups;
            _mapper = mapper;
            _repoInquilinos = repoInquilinos;
            _repoFracoes = repoFracoes;
        }


        /// <summary>
        /// Insere pagamento de renda na base de dados
        /// </summary>
        /// <param name="novoRecebimento">DTO de insert</param>
        /// <param name="isBatchProcessing">Informa se o processamento é em batch ou manual</param>
        /// <returns>1 = ok (commit), -1 = erro no processamento (rollback)</returns>
        public async Task<int> InsertRecebimento(NovoRecebimento novoRecebimento, bool isBatchProcessing = false)
        {
            var IdFracao = novoRecebimento.ID_Propriedade;
            var IsPagamentoRenda = novoRecebimento.Renda;
            string? tipoRecebimento;

            if (IsPagamentoRenda == false)
            {
                tipoRecebimento = _repoLookups.GetDescricao(novoRecebimento.ID_TipoRecebimento, "TipoRecebimento");
            }
            else
            {
                tipoRecebimento = "Pagamento de renda";
            }

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        var paid = 1;
                        var partlyPaid = 2;
                        var fullyPaid = 3;
                        var rentPayment = 99;

                        var paymentStatus = novoRecebimento!.ValorPrevisto == novoRecebimento.ValorRecebido &&
                            novoRecebimento.ValorRecebido > 0 ? paid : novoRecebimento.ValorEmFalta > 0 ? partlyPaid : fullyPaid;

                        novoRecebimento.Estado = paymentStatus;

                        var transactionId = await connection.QuerySingleAsync<int>("usp_Recebimentos_Insert",
                            param: novoRecebimento,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        if (IsPagamentoRenda)
                        {
                            var leaseStatusDescription = novoRecebimento!.ValorPrevisto == novoRecebimento.ValorRecebido &&
                                novoRecebimento.ValorRecebido > 0 ? "Ok" :
                                novoRecebimento.ValorEmFalta > 0 ? "Pagamento parcial" :
                                novoRecebimento.ValorRecebido == 0 ? "Em dívida" : "Should know better...";

                            // Atualiza data do último pagamento + situação do pagamento (contrato)
                            await connection.ExecuteAsync("usp_Arrendamentos_Update_LastPaymentDate",
                                new { Id = IdFracao, date = novoRecebimento.DataMovimento, estadopagamento = leaseStatusDescription },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                        }

                        // Atualiza saldo do inquilino
                        var tenantId = novoRecebimento.ID_Inquilino;

                        var tenantData = await connection.QueryFirstOrDefaultAsync<InquilinoVM>("usp_Inquilinos_GetInquilino_Extended_ById",
                            param: new { Id = tenantId }, commandType: CommandType.StoredProcedure, transaction: tran);

                        var currentBalance = tenantData.SaldoCorrente;
                        var _saldoCorrente = currentBalance + novoRecebimento.ValorRecebido;

                        await connection.ExecuteAsync("usp_Inquilinos_UpdateSaldoCorrente",
                            param: new { TenantId = tenantId, NovoSaldoCorrente = _saldoCorrente },
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        // Cria registo na CC Inquilino
                        CC_InquilinoNovo CC_Inquilino = new()
                        {
                            DataMovimento = novoRecebimento.DataMovimento, // DateTime.Now,
                            IdInquilino = tenantId,
                            ValorPago = novoRecebimento.ValorRecebido,
                            ValorEmDivida = novoRecebimento.ValorEmFalta,
                            Renda = novoRecebimento.Renda,
                            ID_TipoRecebimento = novoRecebimento.Renda ? rentPayment : novoRecebimento.ID_TipoRecebimento,
                            TransactionId = transactionId,
                            Notas = tipoRecebimento
                        };

                        var output = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                            param: CC_Inquilino,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        if (isBatchProcessing)
                        {
                            // Delete temporary payments
                            var deleteOk = await connection.ExecuteAsync("usp_Recebimentos_Delete_Temp",
                                commandType: CommandType.StoredProcedure, transaction: tran);
                        }

                        tran.Commit();
                        _logger.LogInformation("Transação 'InsertRecebimento' terminada com sucesso");

                        return transactionId;
                    }
                }

            }
            catch (SqlException sqlEx)
            {
                _logger.LogError($"Erro na base de dados: {sqlEx.ToString()}");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Processamento mensal de rendas
        /// </summary>
        /// <returns>1 Sucesso, -1 erro</returns>
        public async Task<int> ProcessMonthlyRentPayments()
        {
            int parMonth;
            int parYear;
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            // Lê pagamentos (temp) criados
                            var pagamentosGerados = await connection.QueryAsync<RecebimentoVM>("usp_Recebimentos_Temp_GetAll",
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            parMonth = pagamentosGerados.Select(p => p.DataMovimento.Month).FirstOrDefault();
                            parYear = pagamentosGerados.Select(p => p.DataMovimento.Year).FirstOrDefault();

                            foreach (var pagamento in pagamentosGerados)
                            {
                                var parameters = new DynamicParameters();

                                parameters.Add("@DataMovimento", pagamento.DataMovimento);
                                parameters.Add("@Renda", pagamento.Renda);
                                parameters.Add("@Estado", pagamento.Estado); // default = 1 => pago
                                parameters.Add("@ValorPrevisto", pagamento.ValorPrevisto);
                                parameters.Add("@ValorRecebido", pagamento.ValorRecebido);
                                parameters.Add("@ValorEmFalta", pagamento.ValorEmFalta);
                                parameters.Add("@ID_Propriedade", pagamento.ID_Propriedade);
                                parameters.Add("@ID_TipoRecebimento", pagamento.ID_TipoRecebimento);
                                parameters.Add("@ID_Inquilino", pagamento.ID_Inquilino);
                                parameters.Add("@GeradoPeloPrograma", true);
                                parameters.Add("@Notas", pagamento.Notas);


                                pagamento.GeradoPeloPrograma = true;
                                var idFracao = pagamento.ID_Propriedade;
                                string tipoRecebimento = "Pagamento de renda";
                                var insertTransactionId = await connection.QuerySingleAsync<int>("usp_Recebimentos_Insert",
                                    param: parameters,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                                var status = pagamento!.ValorPrevisto == pagamento.ValorRecebido &&
                                    pagamento.ValorRecebido > 0 ? "Ok" :
                                    pagamento.ValorEmFalta > 0 ? "Pagamento parcial" :
                                    pagamento.ValorRecebido == 0 ? "Em dívida" : "Should know better...";

                                // Atualiza data do último pagamento + situação do pagamento (contrato)
                                await connection.ExecuteAsync("usp_Arrendamentos_Update_LastPaymentDate",
                                    new { Id = idFracao, date = pagamento.DataMovimento, estadopagamento = status },
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                                // Atualiza saldo do inquilino
                                var tenantId = pagamento.ID_Inquilino;

                                var tenantData = await connection.QueryFirstOrDefaultAsync<InquilinoVM>("usp_Inquilinos_GetInquilino_Extended_ById",
                                    param: new { Id = tenantId }, commandType: CommandType.StoredProcedure, transaction: tran);

                                var currentBalance = tenantData.SaldoCorrente;
                                var _saldoCorrente = currentBalance + pagamento.ValorRecebido;

                                // Atualiza saldo corrente do inquilino
                                await connection.ExecuteAsync("usp_Inquilinos_UpdateSaldoCorrente",
                                    param: new { TenantId = tenantId, NovoSaldoCorrente = _saldoCorrente },
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                                // Cria registo na CC Inquilino
                                CC_InquilinoNovo CC_Inquilino = new()
                                {
                                    DataMovimento = pagamento.DataMovimento, // DateTime.Now,
                                    IdInquilino = tenantId,
                                    ValorPago = pagamento.ValorRecebido,
                                    ValorEmDivida = pagamento.ValorEmFalta,
                                    Renda = pagamento.Renda,
                                    ID_TipoRecebimento = pagamento.Renda ? 99 : pagamento.ID_TipoRecebimento,
                                    TransactionId = insertTransactionId,
                                    Notas = tipoRecebimento
                                };

                                var transactionId = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                                    param: CC_Inquilino,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);
                            } // foreach

                            // Get total paid (temp records) por this specific period before deleting them

                            var param2 = new DynamicParameters();

                            param2.Add("@month", parMonth);
                            param2.Add("@year", parYear);
                            param2.Add("totalPaid", dbType: DbType.Decimal, direction: ParameterDirection.Output);

                            await connection.ExecuteAsync("usp_Recebimentos_GetTotalByMonthAndYear",
                                param: param2,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            var totalReceived = (decimal)param2.Get<Decimal>("totalPaid");

                            // Delete temporary payments
                            var deleteOk = await connection.ExecuteAsync("usp_Recebimentos_Delete_Temp",
                                commandType: CommandType.StoredProcedure, transaction: tran);


                            // Register processed month 
                            var result = await connection.ExecuteAsync("usp_Recebimentos_ProcessamentoRendas_Insert",
                                new { month = parMonth, year = parYear, TotalRecebido = totalReceived }, // (-1, porquê?) => os movimentos gerados são para o mês seguinte, o mês processado é o escolhido pelo utilizador
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                            tran.Commit();

                            return 1;

                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError($"Erro no processamento mensal de rendas ({ex.Message}) =>.Rollback...");

                            return -1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no processamento mensal de rendas ({ex.Message}) =>.Rollback...");
                return -1;
            }
        }

        /// <summary>
        /// Cria registo no log de processamento de rendas (após conclusão do processamento mensal)
        /// </summary>
        /// <returns></returns>
        public async Task<int> LogRentProcessingPerformed(NovoProcessamentoRendas record)
        {
            try
            {
                _logger.LogInformation("Sinaliza processamento mensal de rendas (Insert)");

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Recebimentos_ProcessamentoRendas_Insert",
                        param: record,
                        commandType: CommandType.StoredProcedure);

                    return result;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }

        }

        public async Task<bool> AcertaPagamentoRenda(int idRecebimento, int paymentState, decimal valorAcerto)
        {
            var recebimento = await GetRecebimento_ById(idRecebimento);
            var idInquilino = recebimento.ID_Inquilino;
            var inquilino = await _repoInquilinos.GetInquilino_ById(idInquilino);
            var saldoCorrente = inquilino.SaldoCorrente + valorAcerto;
            decimal valorEmFalta = recebimento.ValorEmFalta; // recebimento.ValorPrevisto - valorAcerto; 30/08/2023
            decimal valorRecebido = 0;
            int currentStateAfterPayment = paymentState;
            string? Notas = "";

            int mesMovimento = recebimento.DataMovimento.Month;
            int anoMovimento = recebimento.DataMovimento.Year;

            // TODO - se houver mais de um pagamento em atraso, não atualiza corretamente o saldo do inquilino !! Erro na stored procedure?
            try
            {
                if (paymentState == 2) // parcial
                {
                    valorRecebido = recebimento.ValorRecebido + valorEmFalta;
                    valorEmFalta = 0;
                    Notas = "Acerto de pagamento parcial";
                }
                else // total - pagamento em atraso (3)
                {
                    valorRecebido = valorAcerto;
                    Notas = "Acerto de renda em atraso";
                }

                currentStateAfterPayment = 1;

                var parameters = new DynamicParameters();

                parameters.Add("@Id", idRecebimento);
                parameters.Add("@PaymentState", currentStateAfterPayment);
                parameters.Add("@Notas", Notas);
                parameters.Add("@ValorAcerto", valorAcerto);
                parameters.Add("@ValorEmDivida", valorEmFalta);
                parameters.Add("@SaldoCorrente", saldoCorrente);
                parameters.Add("@ValorRecebido", valorRecebido);
                parameters.Add("@UpdatedDate", DateTime.UtcNow);
                parameters.Add("@TenantId", idInquilino);

                parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);


                using (var connection = _context.CreateConnection())
                {
                    //StringBuilder sb = new();
                    //sb.Append("select * from documentosInquilino ");
                    //sb.Append("WHERE DocumentType = 18 AND ");
                    //sb.Append($"TenantId = {idInquilino} AND ");
                    //sb.Append($"MONTH(ReferralDate) = {mesMovimento} AND ");
                    //sb.Append($"YEAR(ReferralDate) = {anoMovimento}");
                    //var teste = await connection.QueryAsync(sb.ToString());

                    var result = await connection.ExecuteAsync("usp_Recebimentos_AcertaPagamentoRenda",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);

                    bool output = parameters.Get<bool>("@Success");
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }


        public async Task<bool> UpdateRecebimento(AlteraRecebimento alteraRecebimento)
        {
            try
            {
                var tipoTransacao = $"Alteração de {(alteraRecebimento.Renda ? "renda" : "outro recebimento")}";
                _logger.LogInformation(tipoTransacao);

                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            // alteração de renda obriga à criação de registo na CC inquilino e atualização do saldo corrente do inquilino
                            if (alteraRecebimento.Renda)
                            {
                                var tenantId = alteraRecebimento.ID_Inquilino;
                                // Cria registo na CC Inquilino
                                CC_InquilinoNovo CC_Inquilino = new()
                                {
                                    DataMovimento = alteraRecebimento.DataMovimento, // DateTime.Now,
                                    IdInquilino = tenantId,
                                    ValorPago = alteraRecebimento.ValorRecebido,
                                    ValorEmDivida = alteraRecebimento.ValorEmFalta,
                                    Renda = alteraRecebimento.Renda,
                                    ID_TipoRecebimento = alteraRecebimento.Renda ? 99 : alteraRecebimento.ID_TipoRecebimento,
                                    TransactionId = alteraRecebimento.Id,
                                    Notas = "Valor do pagamento foi alterado"
                                };

                                var transactionId = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                                    param: CC_Inquilino,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                                var tenantData = await connection.QueryFirstOrDefaultAsync<InquilinoVM>("usp_Inquilinos_GetInquilino_Extended_ById",
                                    param: new { Id = tenantId }, commandType: CommandType.StoredProcedure, transaction: tran);

                                var currentBalance = tenantData.SaldoPrevisto;
                                var expectedAmount = alteraRecebimento.ValorPrevisto;
                                var paidAmount = alteraRecebimento.ValorRecebido;
                                var difference = expectedAmount - paidAmount;
                                var _saldoCorrente = currentBalance - difference;

                                // atualiza saldo corrente do inquilino
                                await connection.ExecuteAsync("usp_Inquilinos_UpdateSaldoCorrente",
                                    param: new { TenantId = tenantId, NovoSaldoCorrente = _saldoCorrente },
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);


                            } // alteração do valor da renda, criação de movº CC Inquilino

                            // atualiza registo do recebimento
                            var result = await connection.ExecuteAsync("usp_Recebimentos_Update",
                            param: alteraRecebimento,
                            commandType: CommandType.StoredProcedure, transaction: tran);

                            tran.Commit();

                            return result > 0; // > 0 = success

                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError(ex.Message, ex);
                            return false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdateRecebimentoTemp(AlteraRecebimento alteraRecebimento)
        {
            try
            {
                _logger.LogInformation("Atualiza valor do pagamento (temp), antes de o entregar à BD");

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Recebimentos_Temp_Update",
                        param: alteraRecebimento,
                        commandType: CommandType.StoredProcedure);
                    return result > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Delete payment (due rent)
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>true if success (commit), false if failed (rollback)</returns>
        public async Task<bool> DeleteRecebimento(int id)
        {
            const int RENT_TRANSACTION = 99;
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {

                        // antes de apagar movimento...

                        // - Atualiza saldo corrente do Inquilino
                        // - Cria registo na CC do Inquilino
                        // - Se fôr pagamento de renda, atualiza Data do Último Pagamento (contrato) para o mês anterior,
                        // Estado = 'Pendente de regularização / Pagamento'

                        try
                        {
                            var transaction = await GetRecebimento_ById(id);
                            var tenantId = transaction.ID_Inquilino;
                            var unitId = transaction.ID_Propriedade;
                            var transactionAmount = transaction.ValorRecebido;
                            var tenant = await _repoInquilinos.GetInquilino_ById(tenantId);
                            var newTenantBalance = tenant.SaldoCorrente - transactionAmount;
                            var isPagamentoRenda = transaction.Renda;
                            var lastPayDate = transaction.DataMovimento;

                            // Atualiza saldo corrente do Inquilino
                            await connection.ExecuteAsync("usp_Inquilinos_UpdateSaldoCorrente",
                                param: new { TenantId = tenantId, NovoSaldoCorrente = newTenantBalance },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            // Cria registo na CC Inquilino
                            CC_InquilinoNovo CC_Inquilino = new()
                            {
                                DataMovimento = transaction.DataMovimento, // DateTime.Now,
                                IdInquilino = tenantId,
                                ValorPago = 0,
                                ValorEmDivida = transactionAmount,
                                Renda = transaction.Renda,
                                ID_TipoRecebimento = transaction.Renda ? RENT_TRANSACTION : transaction.ID_TipoRecebimento,
                                TransactionId = transaction.Id,
                                Notas = transaction.Renda ? "Não há registo de pagamento de renda => removido lançamento" : "Recebimento (outro) foi removido"
                            };

                            var transactionId = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                                param: CC_Inquilino,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            if (isPagamentoRenda)
                            {
                                // Atualiza data do último pagamento (contrato) para o mês anterior
                                // TODO - confirmar validade deste procedimento

                                // TODO verificar se data de processamento é inferior à existe (no caso de se ter processado um mês anterior)

                                await connection.ExecuteAsync("usp_Arrendamentos_Update_LastPaymentDate",
                                     new { Id = unitId, date = transaction.DataMovimento.AddMonths(-1), estadopagamento = "Pendente de regularização" },
                                     commandType: CommandType.StoredProcedure,
                                     transaction: tran);

                                // 03/2023
                                // soft delete
                                // Se 'pagamento de renda', não apagar, marcar situacao como 'não paga' (Estado = 3)

                                await connection.ExecuteAsync("usp_Recebimentos_SetAsNotPaid",
                                param: new { Id = id },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                            }
                            else
                            {
                                // outro tipo de pagamento, apaga registo da BD
                                await connection.ExecuteAsync("usp_Recebimentos_Delete",
                                    param: new { Id = id },
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);
                            }

                            tran.Commit();
                            return true;

                        }
                        catch (SqlException sqlEx)
                        {
                            tran.Rollback();
                            _logger.LogError(sqlEx.ToString(), sqlEx);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError(ex.ToString(), ex);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        public async Task DeleteRecebimentosTemp()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Recebimentos_Delete_Temp",
                        commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<List<RecebimentoVM>> GetResumedData()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = $"SELECT * FROM vwRecebimentos";
                List<RecebimentoVM> result = connection
                    .Query<RecebimentoVM>(sql)
                    .ToList();

                return result;
            }
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> TotalRecebimentos(int id = 0)
        {
            using (var connection = _context.CreateConnection())
            {
                var output = await connection.QueryFirstOrDefaultAsync<decimal>("usp_Recebimentos_GetTotalReceived",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);
                return output;
            }
        }

        public async Task<decimal> TotalRecebimentos_Inquilino(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<decimal>("usp_Recebimentos_TotalReceived_ByTenant",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }
        public async Task<decimal> TotalRecebimentosPrevisto_Inquilino(int idInquilino)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<decimal>("usp_Recebimentos_TotalExpected_ByTenant",
                        param: new { Id = idInquilino }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<RecebimentoVM> GetRecebimento_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<RecebimentoVM>("usp_Recebimentos_GetRecebimento_ById",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        public async Task<RecebimentoVM> GetRecebimentoTemp_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<RecebimentoVM>("usp_Recebimentos_GetRecebimento_Temp_ById",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        public async Task<ProcessamentoRendas> GetProcessamentoRendas_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<ProcessamentoRendas>("usp_Recebimentos_ProcessamentoRendas_ById",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }
        public async Task<IEnumerable<RecebimentoVM>> GetAll()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<RecebimentoVM>("usp_Recebimentos_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }
        public async Task<IEnumerable<RecebimentoVM>> GetAllTemp()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<RecebimentoVM>("usp_Recebimentos_Temp_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        /// <summary>
        /// Valor da última renda paga, caso haja pagamentos; senão lê esse valor da fração
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<decimal> GetValorUltimaRendaPaga(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<decimal>("usp_Recebimentos_GetValorUltimaRendaPaga_ById",
                            param: new { Id = id },
                            commandType: CommandType.StoredProcedure);
                    return result;
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> RentalProcessingPerformed(int month, int year)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<int>("usp_Recebimentos_ProcessamentoRendas_Check",
                            param: new { month, year },
                            commandType: CommandType.StoredProcedure);
                    return result > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return true;
            }

        }

        /// <summary>
        /// Gera pagamento de rendas (movimentos temporários)
        /// </summary>
        /// <returns>Lista de pagamentos criados, utilizador necessitará de confirmar valores entrados</returns>
        public async Task<IEnumerable<RecebimentoVM>> GeneratePagamentoRendas(IEnumerable<ArrendamentoVM> arrendamentos, int month, int year, bool allowAutomaticRentUpdate)
        {
            try
            {
                List<NovoRecebimento> recebimentos = new List<NovoRecebimento>();
                List<RecebimentoVM> tempClientPayments = new List<RecebimentoVM>();
                foreach (var arrendamento in arrendamentos)
                {
                    // O código foi substituído, uma vez que o último valor pago, poderá ser diferente do real (pagamento parcial) -- usar sempre o valor da renda que está na fração
                    decimal valorRenda = (await _repoFracoes.GetFracao_ById(arrendamento.ID_Fracao)).ValorRenda;
                    //decimal valorUltimaRendaPaga = await GetValorUltimaRendaPaga(arrendamento.ID_Inquilino); // 0, se não houver pagamentos; nessa situação usa valor do contrato de arrendamento

                    DateTime userRequestedDate = new DateTime(year, month, 1).AddMonths(-1); // new 10-04-2023
                    DateTime dataUltimoPagamento = userRequestedDate; // arrendamento.Data_Pagamento;

                    // verifica se contrato já iniciou
                    var monthsUntilNow = GetMonthDifference(arrendamento.Data_Inicio, DateTime.Now);
                    if (monthsUntilNow == 0) // contrato ainda não iniciou... interrompe sequência
                    {
                        continue;
                    }

                    // TODO - verificar, quando <não> for o primeiro processamento, se a data do movimento (pagamento) é a correta
                    // motivo: quando há um processamento mensal, e se apaga um registo (inquilino não pagou...)
                    // ao ser gerado novo pagamento, a data correta de movimento deverá ser a do último mês pago + 1 (poderão haver mais pagamentos em falta...), e não a corrente);
                    // dessa forma, será mais fácil informar quais os meses que estão em falta para cada inquilino ativo (timespan da última data de pagamento, com a data corrente, p,exº)

                    NovoRecebimento recebimentoTemp = new NovoRecebimento()
                    {
                        DataMovimento = dataUltimoPagamento.AddMonths(1),
                        GeradoPeloPrograma = true,
                        ID_Inquilino = arrendamento.ID_Inquilino,
                        ID_Propriedade = arrendamento.ID_Fracao,
                        Renda = true,
                        Estado = 1, // Pago, por default
                        ID_TipoRecebimento = 99,
                        Notas = $"Pagamento do mês de {dataUltimoPagamento.ToString("MMM/yyyy")}",
                        ValorEmFalta = 0,
                        ValorRecebido = valorRenda,
                        ValorPrevisto = valorRenda
                    };

                    int paymentId = await InsertRecebimentoTemp(recebimentoTemp);

                    recebimentos.Add(recebimentoTemp);
                    var clientPayment = _mapper.Map<RecebimentoVM>(recebimentoTemp);
                    clientPayment.Id = paymentId;
                    clientPayment.Inquilino = await _repoInquilinos.GetNomeInquilino(arrendamento.ID_Inquilino);
                    tempClientPayments.Add(clientPayment);


                    // Verifica se arrendamento tem renda para atualizar
                    // TODO => 11-04-2023 - esta verificação deverá ser removida para 'proceso manual de atualização de rendas'
                    var currentYearAsString = DateTime.Now.Year.ToString();
                    var needForAnUpdate = false;
                    if (arrendamento.Data_Inicio.Month == DateTime.Now.Month)
                    {
                        using (var connection = _context.CreateConnection())
                        {
                            var coefficient = await connection.QueryFirstOrDefaultAsync<float>("usp_Arrendamentos_Get_CurrentRentCoefficient",
                                param: new { Ano = DateTime.Now.Year }, commandType: CommandType.StoredProcedure);

                            valorRenda *= (decimal)coefficient;
                            needForAnUpdate = true;
                        }
                    }


                    // atualiza renda, se mês a pagar for o da vigência do contrato e a aplicação estiver configurada para o permitir
                    if (needForAnUpdate && allowAutomaticRentUpdate)
                    {
                        // para efeito de teste, verificar se carta de atualização foi enviada
                        var updateLetterSent = arrendamento.EnvioCartaAtualizacaoRenda;

                        var unitId = arrendamento.ID_Fracao;
                        _logger.LogWarning($"Atualizado valor renda da fração {unitId}, ver explicação na documentação");
                        using (var connection = _context.CreateConnection())
                        {
                            var fracaoAlterada = await connection.ExecuteAsync("usp_Fracoes_UpdateRentValue",
                            param: new { Id = unitId, NewValue = valorRenda },
                            commandType: CommandType.StoredProcedure);
                        }
                    }
                }

                return tempClientPayments;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }


        public async Task<int> InsertRecebimentoTemp(NovoRecebimento entity)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<int>("usp_Recebimentos_Insert_Temp",
                        param: entity,
                        commandType: CommandType.StoredProcedure);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return -1;
                }
            }
        }

        public async Task<IEnumerable<ProcessamentoRendasDTO>> GetMonthlyRentsProcessed(int year)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<ProcessamentoRendasDTO>("usp_Recebimentos_MonthlyRentsProcessed",
                        new { year },
                        commandType: CommandType.StoredProcedure);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return null;
                }
            }

        }

        public async Task<ProcessamentoRendasDTO> GetLastPeriodProcessed()
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var query = "SELECT COUNT(1) FROM ProcessamentoRendas";
                    var cnt = await connection.QueryFirstAsync<int>(query);
                    if (cnt > 0)
                    {
                        var result = await connection.QueryFirstAsync<ProcessamentoRendasDTO>("usp_Recebimentos_GetLastPeriodProcessed",
                            commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    else
                    {
                        return new ProcessamentoRendasDTO();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, "Último período processado");
                    return new ProcessamentoRendasDTO();
                }
            }

        }


        public async Task<decimal> GetMaxValueAllowed_ManualInput(int idInquilino)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<decimal>("usp_Recebimentos_OutrosRecebimentos_MaxValueAllowed_ByTenant",
                        param: new { Id = idInquilino }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return -1;
            }
        }

        private int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            if (endDate.Date < startDate.Date)
            {
                return 0;
            }

            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

    }
}
