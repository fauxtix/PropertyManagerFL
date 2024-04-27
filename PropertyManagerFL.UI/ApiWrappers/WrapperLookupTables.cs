using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math.EC;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperLookupTables : ILookupTableService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperLookupTables> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClientConfigurationService _httpClientConfigService;



        /// <summary>
        /// Generic wrapper for lookup tables
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        /// <param name="memoryCache"></param>
        /// <param name="httpClientConfigService"></param>
        public WrapperLookupTables(IConfiguration env,
                                   ILogger<WrapperLookupTables> logger,
                                   HttpClient httpClient,
                                   IMapper mapper,
                                   IMemoryCache memoryCache,
                                   HttpClientConfigurationService httpClientConfigService)
        {
            _env = env;
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
            _uri = $"{_env["BaseUrl"]}/LookupTables";
            _memoryCache = memoryCache;
            _httpClientConfigService = httpClientConfigService;
            _httpClientConfigService.ConfigureHttpClient(_httpClient);
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
                //RefreshCache(tabela);
                return retCode.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao inserir registo no API (LookupTables)");
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
                //RefreshCache(tabela);

                return retCode.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao atualizar registo no API (LookupTables)");
                return false;
            }
        }


        public async Task<bool> DeleteRegisto(int id, string tableName)
        {
            try
            {
                var retCode = await _httpClient.DeleteAsync($"{_uri}/DeleteLookupRecord/{id}/{tableName}");
                //RefreshCache(tableName);

                return retCode.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao apagar registo no API (LookupTables");
                return false;
            }

        }

        public async Task<string> GetDescription(int id, string tableName)
        {
            string? description = await _httpClient.GetStringAsync($"{_uri}/GetDescriptionByIdAndTable/{id}/{tableName}");

            return description ?? "";
        }

        public async Task<IEnumerable<LookupTableVM>> GetLookupTableData(string tableName)
        {
            try
            {

                IEnumerable<LookupTableVM>? lookupTableData;
                //lookupTableData = _memoryCache.Get<List<LookupTableVM>>(tableName);
                //if (lookupTableData is null)
                //{
                //    var cacheExpiryOptions = new MemoryCacheEntryOptions
                //    {
                //        AbsoluteExpiration = DateTime.Now.AddHours(1),
                //        Priority = CacheItemPriority.High,
                //        SlidingExpiration = TimeSpan.FromMinutes(30)
                //    };

                lookupTableData = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetAllRecords/{tableName}");
                //    _memoryCache.Set(tableName, lookupTableData, cacheExpiryOptions);
                //}
                return lookupTableData!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (LookupTables/GetLookupTableData)");
                return Enumerable.Empty<LookupTableVM>();
            }

        }

        public async Task<bool> CheckIfRecordExist(string description, string tableName)
        {
            bool descriptionExistInDb = false; ;
            try
            {
                //var data = (await GetLookupTableData(tableName)).ToList();

                //descriptionExistInDb = data.Any(d => d.Descricao == description);

                //if (descriptionExistInDb == false)
                    descriptionExistInDb = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/CheckRecordExist/{description}/{tableName}");

                return descriptionExistInDb;
            }
            catch (Exception exc)
            {
                _logger.LogError($"Erro ao pesquisar Descrição ({tableName} - {exc.Message})");
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
                _logger.LogError(exc, "Erro ao pesquisar API (LookupTables/GetCodByDescricao)");
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
        /// Check if the lookup table foreign key is in use
        /// to see if the table 'tableToCheck' record can be deleted
        /// TODO - implement cache functionality. See 'how-to' in this source file
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
                _logger.LogError(exc, "Erro ao pesquisar API (LookupTables/CheckFKInUse)");
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
                _logger.LogError(exc, "Erro ao pesquisar API (LookupTables/CheckRegistoExist)");
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
                _logger.LogError(exc, "Erro ao pesquisar API (LookupTables/GetLastInsertedId)");
                return -1;
            }
        }

        private void RefreshCache(string lookupTable)
        {
            _memoryCache.Remove(lookupTable);
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(1),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            var lookupTableData = GetLookupTableData(lookupTable); // _repoAppConfigTables.GenericGetAll(lookupTable);
            _memoryCache.Set(lookupTable, lookupTableData, cacheExpiryOptions);
        }

    }
}
