using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using System;

namespace PropertyManagerFL.UI.ApiWrappers;

public class WrapperLetterTemplates : ILetterTemplatesService
{
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperLetterTemplates> _logger;
    private readonly string? _templatesUri;
    private readonly HttpClient _httpClient;

    public WrapperLetterTemplates(HttpClient httpClient, IConfiguration env, ILogger<WrapperLetterTemplates> logger)
    {
        _httpClient = httpClient;
        _env = env;
        _logger = logger;
        _templatesUri = $"{_env["BaseUrl"]}/Templates";

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("ApiKey", _env["ApiKey"]);
    }

    public async Task<string> GetTemplateFromServer(string templateName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_templatesUri}/GetTemplateName/{templateName}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                string? fileName = data.Replace("\"", "").Replace("\\\\", "\\");

                if (fileName is null)
                {
                    return "";
                }
                return fileName;
            }

            return "";

        }
        catch (Exception)
        {
            return "";
        }

    }


    public async Task<IEnumerable<Template>> GetAllTemplatesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Template>>(_templatesUri);
            if (response is null)
            {
                return Enumerable.Empty<Template>();
            }

            return response;
        }
        catch (Exception)
        {
            return Enumerable.Empty<Template>();
        }
    }



    public async Task<Template> GetTemplateByIdAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<Template>($"{_templatesUri}/{id}");
        if (response is null)
        {
            return new Template();
        }

        return response;
    }

    public async Task<int> AddTemplateAsync(Template template)
    {
        var response = await _httpClient.PostAsJsonAsync(_templatesUri, template);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<bool> UpdateTemplateAsync(Template template)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_templatesUri}/{template.Id}", template);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<IList<string>> GetTemplatesFilenamesFromServer()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IList<string>>($"{_templatesUri}/GetTemplatesFilenamesFromServer");
            if (!response!.Any())
            {
                return new List<string>();
            }

            return response ?? new List<string>();
        }
        catch (Exception)
        {
            return new List<string>();
        }


    }
}
