using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class ArrendamentoRepository : IArrendamentoRepository
    {
        private const int RENT_PAYMENT_TYPE = 99;

        private readonly IDapperContext _context;
        private readonly IProprietarioRepository _repoOwner;
        private readonly IImovelRepository _repoProperty;
        private readonly IFracaoRepository _repoFracao;
        private readonly IInquilinoRepository _repoInquilino;
        private readonly IFiadorRepository _repoFiador;
        private readonly IRecebimentoRepository _repoRecebimentos;
        private readonly ITipoRecebimentoRepository _repoTipoRecebimentos;
        private readonly ILogger<ArrendamentoRepository> _logger;

        public ArrendamentoRepository(IInquilinoRepository repoInquilino,
                                IRecebimentoRepository repoRecebimentos,
                                ITipoRecebimentoRepository repoTipoRecebimentos,
                                IDapperContext context,
                                ILogger<ArrendamentoRepository> logger,
                                IFracaoRepository repoFracao,
                                IProprietarioRepository repoOwner,
                                IImovelRepository repoProperty,
                                IFiadorRepository repoFiador)
        {
            _repoInquilino = repoInquilino;
            _repoRecebimentos = repoRecebimentos;
            _repoTipoRecebimentos = repoTipoRecebimentos;
            _context = context;
            _logger = logger;
            _repoFracao = repoFracao;
            _repoOwner = repoOwner;
            _repoProperty = repoProperty;
            _repoFiador = repoFiador;
        }

        /// <summary>
        /// Cria contrato de arrendamento
        /// </summary>
        /// <param name="arrendamento">dados do contrato</param>
        /// <returns>1: success, -1: failed</returns>
        public async Task<int> InsertArrendamento(NovoArrendamento arrendamento)
        {
            
            var tenantId = arrendamento.ID_Inquilino;
            var unitId = arrendamento.ID_Fracao;

            var parameters = new DynamicParameters();

            parameters.Add("@Data_Inicio", arrendamento.Data_Inicio);
            parameters.Add("@Data_Fim", arrendamento.Data_Fim);
            parameters.Add("@Data_Pagamento", arrendamento.Data_Pagamento);
            parameters.Add("@SaldoInicial", arrendamento.SaldoInicial);
            parameters.Add("@Fiador", arrendamento.Fiador);
            parameters.Add("@Prazo_Meses", arrendamento.Prazo_Meses);
            parameters.Add("@Prazo", arrendamento.Prazo);
            parameters.Add("@Valor_Renda", arrendamento.Valor_Renda);
            parameters.Add("@Doc_IRS", arrendamento.Doc_IRS);
            parameters.Add("@Doc_Vencimento", arrendamento.Doc_Vencimento);
            parameters.Add("@Notas", arrendamento.Notas);
            parameters.Add("@ID_Fracao", arrendamento.ID_Fracao);
            parameters.Add("@ID_Inquilino", arrendamento.ID_Inquilino);
            parameters.Add("@ID_Fiador", arrendamento.ID_Fiador);
            parameters.Add("@Caucao", arrendamento.Caucao);
            parameters.Add("@ContratoEmitido", arrendamento.ContratoEmitido);
            parameters.Add("@DocumentoGerado", arrendamento.DocumentoGerado);
            parameters.Add("@Data_Saida", arrendamento.Data_Saida);
            parameters.Add("@FormaPagamento", arrendamento.FormaPagamento);
            parameters.Add("@Ativo", arrendamento.Ativo);
            parameters.Add("@LeiVigente", arrendamento.LeiVigente);
            parameters.Add("@ArrendamentoNovo", arrendamento.ArrendamentoNovo);
            parameters.Add("@EstadoPagamento", arrendamento.EstadoPagamento);
            parameters.Add("@RenovacaoAutomatica", arrendamento.RenovacaoAutomatica);

            decimal guarantorSecurity = 0;
            decimal valorRecebido = arrendamento.Valor_Renda;
            int tipoRecebimento = RENT_PAYMENT_TYPE;
            string descricaoMovimento = "";

            try
            {

                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            arrendamento.Data_Pagamento = arrendamento.ArrendamentoNovo ?
                                arrendamento.Data_Inicio.AddMonths(1) :
                                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 8).AddMonths(1);

                            var insertedId = await connection.QueryFirstAsync<int>("usp_arrendamentos_Insert",
                                param: parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);


                            if (arrendamento.ArrendamentoNovo)
                            {
                                // Cria pagamentos iniciais, um para 'renda paga' e outro para 'caução'

                                // pagamento de renda
                                descricaoMovimento = "Pagamento da renda na celebração do contrato";

                                NovoRecebimento recebimento = new NovoRecebimento
                                {
                                    DataMovimento = arrendamento.Data_Inicio,
                                    ValorRecebido = arrendamento.Valor_Renda,
                                    ValorPrevisto = arrendamento.Valor_Renda,
                                    ValorEmFalta = 0,
                                    Estado = 1, // pago
                                    Renda = true,
                                    Notas = descricaoMovimento,
                                    GeradoPeloPrograma = true,
                                    ID_Propriedade = unitId,
                                    ID_TipoRecebimento = tipoRecebimento,
                                    ID_Inquilino = arrendamento.ID_Inquilino
                                };

                                await connection.QuerySingleAsync<int>("usp_Recebimentos_Insert",
                                    recebimento,
                                    commandType: CommandType.StoredProcedure,
                                transaction: tran);


                                // pagamento de caução -- é sempre criado registo, tenha sido escolhida a opção ou não
                                descricaoMovimento = "Pagamento do valor da caução na celebração do contrato";

                                // não foi escolhida opção, cria registo com valor em dívida
                                if (arrendamento.Caucao == false)
                                {
                                    guarantorSecurity = arrendamento.Valor_Renda;
                                    valorRecebido = 0;
                                }

                                DateTime dPagCaucao = arrendamento.Data_Inicio.AddMonths(1);
                                recebimento = new NovoRecebimento
                                {
                                    DataMovimento = dPagCaucao,
                                    ValorRecebido = valorRecebido,
                                    ValorPrevisto = arrendamento.Valor_Renda,
                                    ValorEmFalta = guarantorSecurity,
                                    Estado = 1, // pago (caução
                                    Renda = false,
                                    Notas = descricaoMovimento,
                                    GeradoPeloPrograma = true,
                                    ID_Propriedade = unitId,
                                    ID_TipoRecebimento = _repoTipoRecebimentos.GetID_ByDescription("Caução"),
                                    ID_Inquilino = arrendamento.ID_Inquilino
                                };


                                await connection.QuerySingleAsync<int>("usp_Recebimentos_Insert",
                                     param: recebimento,
                                     commandType: CommandType.StoredProcedure,
                                     transaction: tran);

                            } // Contrato novo

                            // Marca fração como alugada
                            var result = await connection.ExecuteAsync("usp_Fracoes_SetAsRented",
                                 new { Id = unitId },
                                 commandType: CommandType.StoredProcedure, transaction: tran);


                            // Cria registo na CC Inquilino (Id devolvido serve para efeitos de debug)
                            if (arrendamento.ArrendamentoNovo)
                            {
                                CC_InquilinoNovo transaction = new()
                                {
                                    DataMovimento = DateTime.Now,
                                    IdInquilino = tenantId,
                                    ValorPago = valorRecebido * 2,
                                    ValorEmDivida = 0,
                                    Notas = "Pagamentos iniciais"
                                };
                                var IdCreated_CC = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                                    param: transaction,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);
                            }

                            else // existing lease
                            {
                                if (arrendamento.SaldoInicial > 0) // amount due
                                {
                                    CC_InquilinoNovo transaction = new()
                                    {
                                        DataMovimento = DateTime.Now,
                                        IdInquilino = tenantId,
                                        ValorPago = 0,
                                        ValorEmDivida = arrendamento.SaldoInicial,
                                        Notas = "Saldo à data (negativo)"
                                    };

                                    var IdCreated_CC = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                                        param: transaction,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: tran);
                                }

                                // Update tenant balance
                                var tenantBalance = guarantorSecurity; // > 0 if guarantor's security not paid
                                if (arrendamento.SaldoInicial > 0) // is tenant balance (existing lease) negative?
                                    tenantBalance += arrendamento.SaldoInicial;

                                if (tenantBalance > 0)
                                {
                                    var updateParameters = new DynamicParameters();
                                    updateParameters.Add("@TenantId", tenantId);
                                    updateParameters.Add("@NovoSaldoCorrente", -tenantBalance, dbType: DbType.Decimal); // negative entry (there are due values)

                                    await connection.ExecuteScalarAsync("usp_Inquilinos_UpdateSaldoCorrente",
                                       param: updateParameters,
                                       commandType: CommandType.StoredProcedure,
                                       transaction: tran);
                                }
                            }

                            tran.Commit();

                            return insertedId; // Lease Id created
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError(ex.Message);
                            return -1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<bool> UpdateArrendamento(AlteraArrendamento arrendamento)
        {

            var parameters = new DynamicParameters();

            parameters.Add("@Id", arrendamento.Id);
            parameters.Add("@Data_Inicio", arrendamento.Data_Inicio);
            parameters.Add("@Data_Fim", arrendamento.Data_Fim);
            parameters.Add("@Fiador", arrendamento.Fiador);
            parameters.Add("@Prazo_Meses", arrendamento.Prazo_Meses);
            parameters.Add("@Prazo", arrendamento.Prazo);
            parameters.Add("@Valor_Renda", arrendamento.Valor_Renda);
            parameters.Add("@Doc_IRS", arrendamento.Doc_IRS);
            parameters.Add("@Doc_Vencimento", arrendamento.Doc_Vencimento);
            parameters.Add("@Notas", arrendamento.Notas);
            parameters.Add("@ID_Fracao", arrendamento.ID_Fracao);
            parameters.Add("@ID_Inquilino", arrendamento.ID_Inquilino);
            parameters.Add("@ID_Fiador", arrendamento.ID_Fiador);
            parameters.Add("@Caucao", arrendamento.Caucao);
            parameters.Add("@ContratoEmitido", arrendamento.ContratoEmitido);
            parameters.Add("@DocumentoGerado", arrendamento.DocumentoGerado);
            parameters.Add("@Data_Saida", arrendamento.Data_Saida);
            parameters.Add("@FormaPagamento", arrendamento.FormaPagamento);
            parameters.Add("@Ativo", arrendamento.Ativo);
            parameters.Add("@LeiVigente", arrendamento.LeiVigente);
            parameters.Add("@ArrendamentoNovo", arrendamento.ArrendamentoNovo);
            parameters.Add("@EstadoPagamento", arrendamento.EstadoPagamento);
            parameters.Add("@RenovacaoAutomatica", arrendamento.RenovacaoAutomatica);


            try
            {
                _logger.LogInformation("Atualiza dados do arrendamento");

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_arrendamentos_Update",
                        param: parameters,
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
        /// Delete Lease
        /// </summary>
        /// <param name="id">Lease Id</param>
        /// <returns></returns>
        public async Task DeleteArrendamento(int id)
        {
            _logger.LogInformation($"Apaga arrendamento com o id {id} e liberta fração (livre)");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            // get lease's unit id
                            int unitId = await connection.QueryFirstOrDefaultAsync<int>("usp_Arrendamentos_GetUnitId",
                                new { Id = id },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            // delete lease
                            var opCode = await connection.ExecuteAsync("usp_arrendamentos_Delete", param: new { Id = id },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            // set unit as free
                            await connection.ExecuteAsync("usp_Fracoes_SetAsFree", param: new { Id = unitId },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError(ex.Message, ex);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        /// <summary>
        /// Antes de criar um contrato de arrendamento, verificar se os dados necessários foram criados na aplicação
        /// 1. Dados sobre o Proprietário, Imóvel, Fração e InquinoFiador
        /// </summary>
        /// <returns>boolean</returns>
        public async Task<bool> RequirementsMet()
        {
            var ownerExist = await _repoOwner.TableHasData();
            var propertyCreated = await _repoProperty.TableHasData();
            var unitCreated = await _repoFracao.TableHasData();
            var tenantCreated = await _repoInquilino.TableHasData();
            if (tenantCreated)
            {
                if (await _repoFiador.TableHasData() == false) // tenant created but with no guarantor... (required)
                {
                    tenantCreated = false;
                }
            }

            return ownerExist && propertyCreated && unitCreated && tenantCreated;
        }

        public async Task<List<ArrendamentoVM>> GetResumedData()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = "SELECT * FROM vwArrendamentos";
                var result = (await connection.QueryAsync<ArrendamentoVM>(sql)).ToList();
                return result;
            }
        }

        public async Task<IEnumerable<ArrendamentoVM>> GetAll()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ArrendamentoVM>("usp_arrendamentos_GetAll", commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<string> GetNomeInquilino(int Id)
        {
            return await _repoInquilino.GetNomeInquilino(Id);
        }

        public async Task<int> GetLastId()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstAsync<int>("usp_arrendamentos_GetLastId", commandType: CommandType.StoredProcedure);
                return result;
            }
        }
        public async Task UpdateLastPaymentDate(int id, DateTime date)
        {
            try
            {
                _logger.LogInformation("Atualiza última data de pagamento (arrendamento)");

                using (var connection = _context.CreateConnection())
                {
                    await connection.QueryFirstAsync<int>("usp_Arrendamentos_Update_LastPaymentDate",
                        new { Id = id, date },
                        commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public async Task<bool> ChildrenExists(int idFracao)
        {
            _logger.LogInformation("Verifica se arrendamento tem pagamentos efetuados");

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleAsync<int>("usp_arrendamentos_CheckForPayments", new { Id = idFracao }, commandType: CommandType.StoredProcedure);
                return result > 0;
            }
        }

        public async Task GeraMovimentos(Arrendamento arrendamento, int IdFracao)
        {
            try
            {
                _logger.LogInformation("Gera pagamentos iniciais (2), após celebração do contrato de arrendamento");

                NovoRecebimento recebimento = new NovoRecebimento
                {
                    DataMovimento = arrendamento.Data_Inicio,
                    ValorRecebido = arrendamento.Valor_Renda,
                    ID_Propriedade = IdFracao,
                    ID_TipoRecebimento = _repoTipoRecebimentos.GetID_ByDescription("Pagamento de Renda"),
                    ValorEmFalta = 0,
                    Notas = "Pagamento da renda na celebração do contrato",
                    GeradoPeloPrograma = true,
                    ID_Inquilino = arrendamento.ID_Inquilino
                };

                await _repoRecebimentos.InsertRecebimento(recebimento);

                DateTime dPagCaucao = arrendamento.Data_Inicio.AddMonths(1);
                recebimento = new NovoRecebimento
                {
                    DataMovimento = dPagCaucao,
                    ValorRecebido = arrendamento.Valor_Renda,
                    ID_Propriedade = IdFracao,
                    ID_TipoRecebimento = _repoTipoRecebimentos.GetID_ByDescription("Caução"),
                    ValorEmFalta = 0,
                    Notas = "Pagamento do valor da caução na celebração do contrato",
                    GeradoPeloPrograma = true,
                    ID_Inquilino = arrendamento.ID_Inquilino
                };

                await _repoRecebimentos.InsertRecebimento(recebimento);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public string GetDocumentoGerado(int Id)
        {
            _logger.LogInformation("Get contrato gerado para consulta");

            using (var connection = _context.CreateConnection())
            {
                string DocGerado = connection.QuerySingleOrDefault<string>("usp_Arrendamentos_GetGenerated_Document",
                    param: new { Id },
                    commandType: CommandType.StoredProcedure);
                return DocGerado;
            }
        }

        public async Task<bool> ContratoEmitido(int Id)
        {
            _logger.LogInformation("Verifica se Contrato foi emitido");

            // usp_Arrendamentos_Contract_Issued
            using (var connection = _context.CreateConnection())
            {
                bool contratoEmitido = await connection.QuerySingleOrDefaultAsync<bool>("usp_Arrendamentos_Was_Lease_Issued",
                    param: new { Id },
                    commandType: CommandType.StoredProcedure);
                return contratoEmitido;
            }
        }
        public async Task<bool> CartaAtualizacaoRendasEmitida(int ano)
        {
            try
            {
                _logger.LogInformation("Verifica se Carta de Atualização de Rendas foi emitida");

                using (var connection = _context.CreateConnection())
                {
                    bool AtualizacaoRendasProcessada = await connection.QuerySingleOrDefaultAsync<bool>("usp_Arrendamentos_Was_UpdateRentsLetter_Issued",
                        param: new { Ano = ano },
                        commandType: CommandType.StoredProcedure);
                    return AtualizacaoRendasProcessada;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return true;
            }
        }

        public void MarcaContratoComoEmitido(int Id, string docGerado)
        {
            _logger.LogInformation("Marca contrato como emitido");

            using (var connection = _context.CreateConnection())
            {
                connection.Execute("usp_Arrendamentos_SetLease_As_Issued",
                    param: new { Id, GeneratedDocument = docGerado },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> RegistaCartaRevogacao(int id, string docGerado)
        {
            try
            {
                var lease = await GetArrendamento_ById(id);
                var tenantId = lease.ID_Inquilino;

                var parameters = new DynamicParameters();
                parameters.Add("@Descricao", "Carta de oposição à renovação do contrato");
                parameters.Add("@DocumentPath", docGerado);
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@DocumentType", 17);
                parameters.Add("@StorageType", 'S');
                parameters.Add("@StorageFolder", "OposicaoRenovacaoContrato");

                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            _logger.LogInformation("Insere documento de Carta de Revogação/Oposição nos documentos do Inquilino");

                            await connection.ExecuteAsync("usp_Inquilinos_InsertDocument",
                                param: parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            _logger.LogInformation("Regista Carta de Revogação/Oposição");

                            var letterParameters = new DynamicParameters();
                            letterParameters.Add("@Id", id);
                            letterParameters.Add("@EnvioCartaRevogacao", true);
                            letterParameters.Add("@DocumentoRevogacaoGerado", docGerado);
                            letterParameters.Add("@DataEnvioCartaRevogacao", DateTime.UtcNow);
                            await connection.ExecuteAsync("usp_Arrendamentos_Update_RevocationLetter",
                                param: letterParameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            tran.Commit();
                            return true;

                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
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

        public async Task<bool> RegistaProcessamentoAtualizacaoRendas()
        {
            try
            {
                _logger.LogInformation("Regista Processamento de Atualização de Rendas (para o mês corrente) como efetuado");

                var parameters = new DynamicParameters();
                parameters.Add("@Ano", DateTime.UtcNow.Year);
                parameters.Add("@DataProcessamento", DateTime.UtcNow);

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Arrendamentos_Insert_UpdateRentsLetterProcess",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);

                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> VerificaSeExisteCartaRevogacao(int id)
        {
            try
            {
                _logger.LogInformation("Verifica se Carta de Revogação/Oposição foi emitida");

                //var lease = await GetArrendamento_ById(id);
                //var tenantId = lease.ID_Inquilino;

                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QuerySingleOrDefaultAsync<int>("usp_Arrendamentos_Was_RevocationLetter_Issued",
                        param: new { Id = id },
                        commandType: CommandType.StoredProcedure);

                    return output == 1;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> VerificaSeExisteRespostaCartaRevogacao(int id)
        {
            try
            {
                _logger.LogInformation("Verifica se Carta de Revogação/Oposição, foi respondida pelo Inquilino");

                var lease = await GetArrendamento_ById(id);
                var answered = lease.RespostaCartaRevogacao;

                return answered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> RegistaCartaAtraso(int id, DateTime? referralDate, string tentativa, string docGerado)
        {

            try
            {
                _logger.LogInformation("Regista carta de atraso no pagamento de rendas");

                var lease = await GetArrendamento_ById(id);
                var tenantId = lease.ID_Inquilino;

                var parameters = new DynamicParameters();
                parameters.Add("@Descricao", $"Carta de atraso no pagamento de rendas {tentativa}");
                parameters.Add("@DocumentPath", docGerado);
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@ReferralDate", referralDate);
                parameters.Add("@DocumentType", 18);
                parameters.Add("@StorageType", 'S');
                parameters.Add("@StorageFolder", "RendasAtraso");

                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            // Cria registo em 'Documentos do Inquilino'
                            await connection.ExecuteAsync("usp_Inquilinos_InsertDocument",
                                param: parameters,
                                commandType:CommandType.StoredProcedure,
                                transaction: tran);

                            // Atualiza flag de 'carta de atraso enviada' na tabela de arrendamentos
                            var letterParameters = new DynamicParameters();
                            letterParameters.Add("@Id", id);
                            letterParameters.Add("@EnvioCartaAtrasoRenda", true);
                            letterParameters.Add("@DocumentoAtrasoRendaGerado", docGerado);
                            letterParameters.Add("@DataEnvioCartaAtrasoRenda", DateTime.UtcNow);
                            await connection.ExecuteAsync("usp_Arrendamentos_Update_LateRentPaymentLetter",
                                param: letterParameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            tran.Commit();

                            return true;

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error {ex.Message} - Rollback... ");
                            tran.Rollback();
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

        public async Task<bool> VerificaEnvioCartaAtrasoEfetuado(int id)
        {
            try
            {

                var lease = await GetArrendamento_ById(id);
                var letterAlreadySent = lease.EnvioCartaAtrasoRenda;

                return letterAlreadySent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }


        public async Task<bool> MarcaCartaAtrasoRendaComoEmitida(int id, string docGerado)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var lease = await GetArrendamento_ById(id);
                        var tenantId = lease.ID_Inquilino;

                        _logger.LogInformation("Marca carta de atraso de renda como emitida");

                        var result = await connection.ExecuteAsync("usp_Arrendamentos_SetLateRentPaymentLetter_Issued",
                            param: new { Id = id, GeneratedDocument = docGerado },
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        // inclui documento nos 'documentos do inquilino'

                        var parameters = new DynamicParameters();
                        parameters.Add("@Descricao", "Carta de atraso de renda");
                        parameters.Add("@DocumentPath", docGerado);
                        parameters.Add("@TenantId", tenantId);
                        parameters.Add("@DocumentType", 18);
                        parameters.Add("@StorageType", 'S');
                        parameters.Add("@StorageFolder", "AtrasoRendas");


                        await connection.ExecuteAsync("usp_Inquilinos_InsertDocument",
                            param: parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        tran.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        public async Task<bool> MarcaCartaAtualizacaoComoEmitida(int id, string docGerado)
        {

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var lease = await GetArrendamento_ById(id);
                        var tenantId = lease.ID_Inquilino;
                        var unitId = lease.ID_Fracao;
                        var unit = await _repoFracao.GetFracao_ById(unitId);
                        var currentRent = unit.ValorRenda;

                        var currentYearAsString = DateTime.Now.Year.ToString();
                        var coefficient = await GetCurrentRentCoefficient(currentYearAsString);

                        var newRent = currentRent * (decimal)coefficient;

                        _logger.LogInformation("Marca carta de atualização de renda como emitida");

                        var result = await connection.ExecuteAsync("usp_Arrendamentos_SetUpdateRentsLetter_Issued",
                            param: new { Id = id, GeneratedDocument = docGerado },
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        // inclui documento nos 'documentos do inquilino'
                        _logger.LogInformation("Após envio de carta de atualização de renda, incluir documento nos 'documentos do inquilino'");

                        var parameters = new DynamicParameters();
                        parameters.Add("@Descricao", "Carta de atualização de renda");
                        parameters.Add("@DocumentPath", docGerado);
                        parameters.Add("@TenantId", tenantId);
                        parameters.Add("@DocumentType", 16);
                        parameters.Add("@StorageType", 'S');
                        parameters.Add("@StorageFolder", "AtualizacaoRendas");


                        await connection.ExecuteAsync("usp_Inquilinos_InsertDocument",
                            param: parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        tran.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        public async Task<int> InsertRentCoefficient(CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas)
        {
            _logger.LogInformation("Insere novo coeficiente de atualização de rendas na BD'");
            var coef = coeficienteAtualizacaoRendas.Coeficiente;
            var newCoef = Convert.ToDouble(coef);

            var parameters = new DynamicParameters();
            parameters.Add("@Ano", coeficienteAtualizacaoRendas.Ano);
            parameters.Add("@Coeficiente", newCoef);
            parameters.Add("@DiplomaLegal", coeficienteAtualizacaoRendas.DiplomaLegal);
            parameters.Add("@UrlDiploma", coeficienteAtualizacaoRendas.UrlDiploma);
            parameters.Add("@Lei", coeficienteAtualizacaoRendas.Lei);
            parameters.Add("@DataPublicacao", coeficienteAtualizacaoRendas.DataPublicacao);

            using (var connection = _context.CreateConnection())
            {
                var recordId = await connection.QueryFirstOrDefaultAsync<int>("usp_Arrendamentos_Insert_RentCoefficient",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return recordId;
            }
        }
        public async Task<bool> UpdateRentCoefficient(CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas)
        {
            try
            {
                _logger.LogInformation("Atualiza coeficiente de atualização de rendas");

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Arrendamentos_Update_RentCoefficient",
                        param: coeficienteAtualizacaoRendas,
                        commandType: CommandType.StoredProcedure);
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<CoeficienteAtualizacaoRendas>> GetRentUpdatingCoefficients()
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<CoeficienteAtualizacaoRendas>("usp_Arrendamentos_Get_RentCoefficients",
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }
        public async Task<CoeficienteAtualizacaoRendas> GetRentUpdatingCoefficient_ById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<CoeficienteAtualizacaoRendas>("usp_Arrendamentos_Get_RentCoefficient_ById",
                    new { Id = id }, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<CoeficienteAtualizacaoRendas> GetCoefficient_ByYear(int year)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<CoeficienteAtualizacaoRendas>("usp_Arrendamentos_Get_RentCoefficient_ByYear",
                    new { Year = year }, commandType: CommandType.StoredProcedure);
                return result;
            }
        }


        public void MarcaContratoComoNaoEmitido(int Id)
        {
            _logger.LogInformation("Marca contrato como não emitido");

            using (var connection = _context.CreateConnection())
            {
                connection.Execute(" usp_Arrendamentos_SetLease_NotIssued",
                    param: new { Id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public decimal TotalRendas()
        {
            _logger.LogInformation("Get total de rendas pagas");

            using (var connection = _context.CreateConnection())
            {
                decimal decTotal = connection.QuerySingleOrDefault<decimal>("usp_Arrendamentos_IncomeReceived",
                    commandType: CommandType.StoredProcedure);
                return decTotal;
            }
        }

        public void CriaRegistoHistorico(Arrendamento arrendamento)
        {
            HistoricoAtualizacaoRenda histArrendamento;

        }

        public bool RenovacaoAutomatica(int Id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    bool RenAutomatica = connection.QuerySingleOrDefault<bool>("usp_Arrendamentos_AutoRenewed",
                        param: new { Id },
                        commandType: CommandType.StoredProcedure);
                    return RenAutomatica;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CheckNewRents() // TODO create stored procedures
        {
            var date = DateTime.Now.AddDays(10);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            connection.Open();
                            var arrendamentos = connection.Query<Arrendamento>("SELECT Id FROM Arrendamento").ToList()
                                .Where(c => date >= c.Data_Pagamento).ToList();
                            foreach (var arrendamento in arrendamentos)
                            {
                                connection.Execute("UPDATE Arrendamento SET EstadoPagamento = 'Pendente' WHERE Id = @Id",
                                    new { arrendamento.Id }, transaction: tran);
                            }

                            tran.Commit();

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message, ex);
                            tran.Rollback();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ArrendamentoExiste(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                bool exist = connection.QuerySingleOrDefault<bool>("usp_Arrendamentos_RentalExist",
                    param: new { Id = id },
                    commandType: CommandType.StoredProcedure);
                return exist;
            }
        }

        public async Task<ArrendamentoVM> GetArrendamento_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<ArrendamentoVM>("usp_Arrendamentos_GetById",
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

        public async Task<DateTime> GetLastPaymentDate(int unitId)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<DateTime>("usp_Arrendamentos_Get_LastPaymentDate",
                        param: new { Id = unitId }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return DateTime.MinValue;
            }
        }
        public async Task<float> GetCurrentRentCoefficient(string? ano)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<float>("usp_Arrendamentos_Get_CurrentRentCoefficient",
                        param: new { Ano = ano }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task ExtendLeaseTerm(int Id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.ExecuteAsync("usp_Arrendamentos_ExtendLeaseTerm",
                      new { Id },
                      commandType: CommandType.StoredProcedure);
                    return;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetApplicableLaws()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<LookupTableVM>("usp_Arrendamentos_Get_ApplicableLaws",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<LookupTableVM>();
            }

        }

        public async Task<int> GetIdInquilino(int tenantId)
        {
            throw new NotImplementedException();
        }
    }
}
