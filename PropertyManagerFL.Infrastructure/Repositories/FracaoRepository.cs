using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.SituacaoFracao;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class FracaoRepository : IFracaoRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<FracaoRepository> _logger;

        public FracaoRepository(DapperContext context, ILogger<FracaoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsereFracao(NovaFracao fracao, List<NovaImagemFracao> imagensFracao)
        {
            var fracaoHasPhotos = imagensFracao is not null;

            var parameters = new DynamicParameters();
            parameters.Add("@Ativa", fracao.Ativa);
            parameters.Add("@Descricao", fracao.Descricao);
            parameters.Add("@ValorRenda", fracao.ValorRenda);
            parameters.Add("@AreaBrutaPrivativa", fracao.AreaBrutaPrivativa);
            parameters.Add("@AreaBrutaDependente", fracao.AreaBrutaDependente);
            parameters.Add("@CasasBanho", fracao.CasasBanho);
            parameters.Add("@Varanda", fracao.Varanda);
            parameters.Add("@Terraco", fracao.Terraco);
            parameters.Add("@Garagem", fracao.Garagem);
            parameters.Add("@Arrecadacao", fracao.Arrecadacao);
            parameters.Add("@GasCanalizado", fracao.GasCanalizado);
            parameters.Add("@CozinhaEquipada", fracao.CozinhaEquipada);
            parameters.Add("@LugarEstacionamento", fracao.LugarEstacionamento);
            parameters.Add("@Fotos", true);
            parameters.Add("@Notas", fracao.Notas);
            parameters.Add("@Tipologia", fracao.Tipologia);
            parameters.Add("@ID_CertificadoEnergetico", fracao.ID_CertificadoEnergetico);
            parameters.Add("@Matriz", fracao.Matriz);
            parameters.Add("@LicencaHabitacao", fracao.LicencaHabitacao);
            parameters.Add("@DataEmissaoLicencaHabitacao", fracao.DataEmissaoLicencaHabitacao);
            parameters.Add("@Andar", fracao.Andar);
            parameters.Add("@Lado", fracao.Lado);
            parameters.Add("@AnoUltAvaliacao", fracao.AnoUltAvaliacao);
            parameters.Add("@ValorUltAvaliacao", fracao.ValorUltAvaliacao);
            parameters.Add("@ID_TipoPropriedade", fracao.ID_TipoPropriedade);
            parameters.Add("@Id_Imovel", fracao.Id_Imovel);
            parameters.Add("@Situacao", fracao.Situacao);
            parameters.Add("@Conservacao", fracao.Conservacao);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        _logger.LogInformation("Insere registo de Fração");

                        var idFracaoCriada = await connection.QuerySingleAsync<int>("usp_Fracoes_Insert",
                            param: parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        if (fracaoHasPhotos)
                        {
                            foreach (var imagem in imagensFracao!)
                            {
                                var imageParameters = new DynamicParameters();
                                imageParameters.Add("@Id_Fracao", idFracaoCriada);
                                imageParameters.Add("@Descricao", imagem.Descricao);
                                imageParameters.Add("@Foto", imagem.Foto);
                                var imageId = await connection.QuerySingleAsync<int>("usp_Fracoes_InsertImage",
                                    param: imageParameters,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: tran);
                            }
                        }

                        tran.Commit();
                        return idFracaoCriada;

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        tran.Rollback();
                        return -1;
                    }
                }
            }
        }

        public async Task<bool> AtualizaFracao(AlteraFracao fracao, List<NovaImagemFracao> imagensFracao)
        {
            var fracaoHasPhotos = imagensFracao is not null;

            var parameters = new DynamicParameters();
            parameters.Add("@Id", fracao.Id);
            parameters.Add("@Ativa", fracao.Ativa);
            parameters.Add("@Descricao", fracao.Descricao);
            parameters.Add("@ValorRenda", fracao.ValorRenda);
            parameters.Add("@AreaBrutaPrivativa", fracao.AreaBrutaPrivativa);
            parameters.Add("@AreaBrutaDependente", fracao.AreaBrutaDependente);
            parameters.Add("@CasasBanho", fracao.CasasBanho);
            parameters.Add("@Varanda", fracao.Varanda);
            parameters.Add("@Terraco", fracao.Terraco);
            parameters.Add("@Garagem", fracao.Garagem);
            parameters.Add("@Arrecadacao", fracao.Arrecadacao);
            parameters.Add("@GasCanalizado", fracao.GasCanalizado);
            parameters.Add("@CozinhaEquipada", fracao.CozinhaEquipada);
            parameters.Add("@LugarEstacionamento", fracao.LugarEstacionamento);
            parameters.Add("@Fotos", true);
            parameters.Add("@Notas", fracao.Notas);
            parameters.Add("@Tipologia", fracao.Tipologia);
            parameters.Add("@ID_CertificadoEnergetico", fracao.ID_CertificadoEnergetico);
            parameters.Add("@Matriz", fracao.Matriz);
            parameters.Add("@LicencaHabitacao", fracao.LicencaHabitacao);
            parameters.Add("@DataEmissaoLicencaHabitacao", fracao.DataEmissaoLicencaHabitacao);
            parameters.Add("@Andar", fracao.Andar);
            parameters.Add("@Lado", fracao.Lado);
            parameters.Add("@AnoUltAvaliacao", fracao.AnoUltAvaliacao);
            parameters.Add("@ValorUltAvaliacao", fracao.ValorUltAvaliacao);
            parameters.Add("@ID_TipoPropriedade", fracao.ID_TipoPropriedade);
            parameters.Add("@Id_Imovel", fracao.Id_Imovel);
            parameters.Add("@Situacao", fracao.Situacao);
            parameters.Add("@Conservacao", fracao.Conservacao);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            _logger.LogInformation("Atualiza registo de Fração");

                            var fracaoAlterada = await connection.QueryAsync("usp_Fracoes_Update",
                                    param: parameters, commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                            if (fracaoHasPhotos)
                            {
                                // Apaga imagens existentes e cria novas
                                _logger.LogInformation("Há fotos da fração. Apaga existentes e cria novas");

                                var imagesDeleted = await connection.ExecuteAsync("usp_Fracoes_DeleteImages",
                                    param: new { Id = fracao.Id }, commandType: CommandType.StoredProcedure,
                                    transaction: tran);

                                foreach (var imagem in imagensFracao!)
                                {
                                    var imageParameters = new DynamicParameters();
                                    imageParameters.Add("@Id_Fracao", fracao.Id);
                                    imageParameters.Add("@Descricao", imagem.Descricao);
                                    imageParameters.Add("@Foto", imagem.Foto);
                                    var imageId = await connection.QuerySingleAsync<int>("usp_Fracoes_InsertImage",
                                        param: imageParameters,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: tran);
                                }
                            }

                            tran.Commit();
                            return fracaoAlterada is not null;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
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

        public async Task ApagaFracao(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            _logger.LogInformation("Apaga imagens da fração antes de remover a mesma");

                            await connection.ExecuteAsync("usp_Fracoes_DeleteImages",
                                param: new { Id = id },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            _logger.LogInformation("Apaga fração");

                            await connection.ExecuteAsync("usp_Fracoes_Delete",
                                param: new { Id = id },
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public async Task<int> InsereImagemFracao(NovaImagemFracao imagem)
        {
            try
            {
                _logger.LogInformation("Insere foto de uma fração");

                var parameters = new DynamicParameters();
                parameters.Add("@Descricao", imagem.Descricao);
                parameters.Add("@Foto", imagem.Foto);
                parameters.Add("@Id_Fracao", imagem.Id_Fracao);
                using (var connection = _context.CreateConnection())
                {
                    var idFracaoCriada = await connection.QuerySingleAsync<int>("usp_Fracoes_InsertImage",
                         param: parameters,
                         commandType: CommandType.StoredProcedure);

                    return idFracaoCriada;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<bool> AtualizaImagemFracao(AlteraImagemFracao imagem)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var fracaoAlterada = await connection.ExecuteAsync("usp_Fracoes_UpdateImage",
                        param: imagem, commandType: CommandType.StoredProcedure);
                    return fracaoAlterada > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        public async Task<bool> AtualizaValorRenda(int id, decimal novoValorRenda)
        {
            try
            {
                _logger.LogInformation("Atualiza valor da renda de uma fração");

                using (var connection = _context.CreateConnection())
                {
                    var fracaoAlterada = await connection.ExecuteAsync("usp_Fracoes_UpdateRentValue",
                        param: new { Id = id, NewValue = novoValorRenda },
                        commandType: CommandType.StoredProcedure);
                    return fracaoAlterada > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task ApagaImagemFracao(int id)
        {
            try
            {
                _logger.LogInformation("Apaga imagem de uma fração");

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Fracoes_DeleteImage",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }


        public async Task<FracaoVM> GetFracao_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var unit = await connection.QueryFirstOrDefaultAsync<FracaoVM>("usp_Fracoes_GetById",
                        param: new { Id = id },
                        commandType: CommandType.StoredProcedure);

                    var unitImages = await GetImages_ByUnitId(id);
                    if (unitImages.Count() > 0)
                        unit.Imagens = unitImages.ToList();

                    return unit;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        // Não usar, sp não serve o propósito requerido
        public async Task<FracaoVM> GetUnit_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<FracaoVM>("usp_Fracoes_GetUnitById",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);

                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<IEnumerable<NovaImagemFracao>> GetImages_ByUnitId(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<NovaImagemFracao>("usp_Fracoes_GetImages_ByUnitId",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);

                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<ImagemFracao> GetImage_ByUnitId(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<ImagemFracao>("usp_Fracoes_GetImage_ByUnitId",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);

                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }



        public int GetFirstId()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP 1 Id FROM Fracoes ");
            sb.Append("ORDER BY Id");

            using (var connection = _context.CreateConnection())
            {
                var output = connection.QueryFirstOrDefault<int>(sb.ToString());
                return output;
            }
        }

        public async Task<List<FracaoVM>> GetResumedData()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = $"SELECT * FROM vwFracoes";
                var result = (await connection.QueryAsync<FracaoVM>(sql)).ToList();
                return result;
            }
        }

        public async Task<IEnumerable<FracaoVM>> GetAll()
        {
            try
            {

                using (var connection = _context.CreateConnection())
                {
                    var list = (await connection.QueryAsync<FracaoVM>("usp_Fracoes_GetAll",
                        commandType: CommandType.StoredProcedure)).ToList();
                    return list;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lookup table de frações
        /// </summary>
        /// <param name="Id">Id do imovel (0 = Todas)</param>
        /// <returns></returns>
        public async Task<IEnumerable<LookupTableVM>> GetFracoes(int id = 0)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var list = (await connection.QueryAsync<LookupTableVM>("usp_Fracoes_GetFracoes_Lookup",
                        param: new { Id = id },
                        commandType: CommandType.StoredProcedure)).ToList();
                    return list;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<FracaoVM>> GetFracoes_Imovel(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var list = (await connection.QueryAsync<FracaoVM>("usp_Fracoes_GetPropertyUnits",
                        param: new { id }, commandType: CommandType.StoredProcedure))
                        .ToList();
                    return list;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        public async Task<bool> MarcaFracaoComoAlugada(int id)
        {
            try
            {

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Fracoes_SetAsRented",
                        new { Id = id },
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

        public async Task<bool> MarcaFracaoComoLivre(int id)
        {
            try
            {
                _logger.LogInformation("Marca fração como livre");

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Fracoes_SetAsFree",
                        new { Id = id },
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


        public async Task<IEnumerable<LookupTableVM>> GetFracoes_ComArrendamentoCriado()
        {
            string sql = $"SELECT * FROM vwFracoesComArrendamentos";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var list = (await connection.QueryAsync<LookupTableVM>(sql)).ToList();
                    return list;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetFracoes_Disponiveis(int IdImovel = 0)
        {

            var sp = "usp_Fracoes_GetAvailable";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<LookupTableVM>(sp, commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetFracoes_WithDuePayments()
        {

            var sp = "usp_Fracoes_GetFracoes_WithDuePayments";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<LookupTableVM>(sp, commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetFracoes_SemContrato(int id)
        {

            var sp = "usp_Fracoes_GetUnitsWithoutLease";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<LookupTableVM>(sp,
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



        /// <summary>
        /// Liberta fração, após fim de contrato
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> MarcaFracaoLivre(int id)
        {
            try
            {
                _logger.LogInformation("Liberta fração, após fim de contrato");

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Fracoes_SetAsFree",
                        new { Id = id },
                        commandType: CommandType.StoredProcedure);
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false; ;
            }
        }

        public async Task<string> GetNomeFracao(int Id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var sqlUpdate = $"SELECT Descricao FROM Fracao WHERE Id = @Id";
                    var nomeFracao = await connection.QueryFirstOrDefaultAsync<string>(sqlUpdate, new { Id });
                    return nomeFracao;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }
        }

        private async Task<int> FindCodigoSituacaoFracao(string Descricao)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int codSituacao = await connection.QueryFirstOrDefaultAsync<int>
                        ("SELECT Id FROM SituacaoFracao WHERE  Descricao = @Descricao", new { Descricao });

                    return codSituacao;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 999;
            }
        }

        public async Task<bool> FracaoEstaLivre(int Id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int situacaoFracao = await connection.QueryFirstOrDefaultAsync<int>
                        ("usp_Fracoes_IsUnitFreeToLease",
                        new { Id },
                        commandType: CommandType.StoredProcedure);

                    return situacaoFracao > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<string> GetDescricaoSituacao(int Id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Descricao ");
            sb.Append("FROM SituacaoFracao ");
            sb.Append("WHERE Id = @Id");
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<string>(sb.ToString(), new { Id });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }
        }

        public async Task<List<SituacaoFracaoVM>> GetSituacaoFracao()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    string sql = $"SELECT * FROM SituacaoFracao";
                    var result = (await connection.QueryAsync<SituacaoFracaoVM>(sql)).ToList(); ;
                    return result;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<int> GetIDSituacao_ByDescription(string descricao)
        {
            var query = "SELECT Id FROM SituacaoFracao WHERE Descricao = @descricao";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int output = await connection.QueryFirstOrDefaultAsync<int>(query, new { descricao });
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 999;
            }
        }
        public async Task<bool> TableHasData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(1) ");
            sb.Append("FROM Fracoes");

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<int>(sb.ToString());
                return result > 0;
            }

        }

    }
}
