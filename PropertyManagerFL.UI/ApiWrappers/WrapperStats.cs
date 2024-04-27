﻿using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.Stats;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperStats : IStatsService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperStats> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly HttpClientConfigurationService _httpClientConfigService;


        public WrapperStats(IConfiguration env, ILogger<WrapperStats> logger, HttpClient httpClient, HttpClientConfigurationService httpClientConfigService)
        {
            _env = env;
            _logger = logger;
            _httpClient = httpClient;
            _uri = $"{_env["BaseUrl"]}/Stats";
            _httpClientConfigService = httpClientConfigService;
            _httpClientConfigService.ConfigureHttpClient(_httpClient);

        }
        public async Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpending()
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetExpensesCategoriesWithMoreSpending"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<ExpensesSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<ExpensesSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<ExpensesSummaryData>();
            }
        }

        public async Task<IEnumerable<ExpensesSummaryData>> GetExpensesCategoriesWithMoreSpendings_ByYear(int year)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetExpensesCategoriesWithMoreSpendings_ByYear/{year}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<ExpensesSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<ExpensesSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<ExpensesSummaryData>();
            }
        }

        public async Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses(int year)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalExpenses/{year}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<ExpensesSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        await Task.Delay(1000);
                        return Enumerable.Empty<ExpensesSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<ExpensesSummaryData>();
            }
        }

        public async Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByType()
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalExpenses_ByType"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<ExpensesSummaryDataByType>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<ExpensesSummaryDataByType>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Stats/GetTotalExpenses_ByTypeAndYear)");
                return Enumerable.Empty<ExpensesSummaryDataByType>();
            }

        }

        public async Task<IEnumerable<ExpensesSummaryDataByType>> GetTotalExpenses_ByTypeAndYear(int year)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalExpenses_ByTypeAndYear/{year}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<ExpensesSummaryDataByType>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<ExpensesSummaryDataByType>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Stats/GetTotalExpenses_ByTypeAndYear)");
                return Enumerable.Empty<ExpensesSummaryDataByType>();
            }
        }

        public async Task<IEnumerable<ExpensesSummaryData>> GetTotalExpenses_ByYear(int year)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalExpenses_ByYear/{year}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<ExpensesSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<ExpensesSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Stats/GetTotalExpenses_ByYear)");
                return Enumerable.Empty<ExpensesSummaryData>();
            }
        }

        public async Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments()
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalPayments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<PaymentsSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<PaymentsSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Stats/GetTotalPayments)");
                return Enumerable.Empty<PaymentsSummaryData>();
            }
        }

        public async Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments_ByPaymentMethod(int year)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalPayments_ByPaymentMethod/{year}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<PaymentsSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<PaymentsSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<PaymentsSummaryData>();
            }
        }

        public async Task<IEnumerable<PaymentsSummaryData>> GetTotalPayments_ByYear(int year)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTotalPayments_ByYear/{year}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<PaymentsSummaryData>>(data);
                        return output.ToList();
                    }
                    else
                    {
                        return Enumerable.Empty<PaymentsSummaryData>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<PaymentsSummaryData>();
            }
        }

        public double Percentage(int current, int iRecords)
        {
            return Math.Round(Convert.ToDouble(((double)current / iRecords) * 100), 2);
        }
    }
}
