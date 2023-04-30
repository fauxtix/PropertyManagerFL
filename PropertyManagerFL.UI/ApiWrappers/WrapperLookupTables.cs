using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperLookupTables : ILookupTableService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperLookupTables> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        /// <summary>
        /// Generic wrapper for lookup tables
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        public WrapperLookupTables(IConfiguration env,
                                   ILogger<WrapperLookupTables> logger,
                                   HttpClient httpClient,
                                   IMapper mapper)
        {
            _env = env;
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
            _uri = $"{_env["BaseUrl"]}/LookupTables";

        }

        /// <summary>
        /// Cria novo registo
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="tabela"></param>
        /// <returns></returns>
        public async Task<bool> CriaNovoRegisto(string descricao, string tabela)
        {
            try
            {
                var descriptionExist = await CheckIfRecordExist(descricao, tabela);
                if (descriptionExist) return false;

                LookupTableVM lookupTable = new LookupTableVM()
                {
                    Descricao = descricao,
                    Tabela = tabela
                };

                var retCode = await _httpClient.PostAsJsonAsync($"{_uri}/InsertRecord", lookupTable);
                return retCode.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao inserir registo no API");
                return false;
            }
        }

        /// <summary>
        /// Update support table
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="descricao"></param>
        /// <param name="tabela"></param>
        /// <returns></returns>
        public async Task<bool> ActualizaDetalhes(int codigo, string descricao, string tabela)
        {
            try
            {
                LookupTableVM lookupTable = new LookupTableVM()
                {
                    Descricao = descricao,
                    Tabela = tabela,
                    Id = codigo
                };

                var retCode = await _httpClient.PutAsJsonAsync($"{_uri}/UpdateLookupTable/{codigo}", lookupTable);
                return retCode.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao atualizar registo no API");
                return false;
            }
        }


        public async Task<bool> DeleteRegisto(int id, string tableName)
        {
            try
            {
                var retCode = await _httpClient.DeleteAsync($"{_uri}/DeleteLookupRecord/{id}/{tableName}");
                return retCode.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao apagar registo no API");
                return false;
            }

        }

        public async Task<string> GetDescription(int id, string tableName)
        {
            var descriprion = await _httpClient.GetStringAsync($"{_uri}/GetDescriptionByIdAndTable/{id}/{tableName}");
            return descriprion;
        }

        public async Task<IEnumerable<LookupTableVM>> GetLookupTableData(string tableName)
        {
            try
            {
                var tableData = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetAllRecords/{tableName}");
                return tableData!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<LookupTableVM>();
            }

        }

        public async Task<bool> CheckIfRecordExist(string description, string tableName)
        {
            try
            {
                var existInDb = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/CheckRecordExist/{description}/{tableName}");
                return existInDb;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return false;
            }
        }



        public async Task<int> GetCodByDescricao(string description, string tableName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/GetPKByDescriptionAndTable/{description}/{tableName}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<int>(data);
                    return output;
                }

                return -1;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return -1;
            }
        }

        public string GetDescricao(int Codigo, string Tabela)
        {
            throw new NotImplementedException();
        }

        public int GetId(string Descricao, string Tabela)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CheckFKInUse
        /// </summary>
        /// <param name="IdFK"></param>
        /// <param name="fieldToCheck"></param>
        /// <param name="tableToCheck"></param>
        /// <returns></returns>
        public async Task<bool> CheckFKInUse(int IdFK, string fieldToCheck, string tableToCheck)
        {
            try
            {
                var existInDb = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/CheckFkInUse/{IdFK}/{fieldToCheck}/{tableToCheck}");
                return existInDb;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return false;
            }
        }

        /// <summary>
        /// Check Registo Exist (redundante) há a versão com 'If'
        /// </summary>
        /// <param name="description"></param>
        /// <param name="tableToCheck"></param>
        /// <returns></returns>
        public async Task<bool> CheckRegistoExist(string description, string tableToCheck)
        {
            try
            {
                var existInDb = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/CheckRecordExist/{description}/{tableToCheck}");
                return existInDb;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return false;
            }
        }

        /// <summary>
        /// Get last inserted Id
        /// </summary>
        /// <param name="tableToCheck"></param>
        /// <returns></returns>
        public async Task<int> GetLastInsertedId(string tableToCheck)
        {
            try
            {
                var lastInsertedId = await _httpClient.GetFromJsonAsync<int>($"{_uri}/GetLastInsertedId/{tableToCheck}");
                return lastInsertedId;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return -1;
            }
        }
    }
}
