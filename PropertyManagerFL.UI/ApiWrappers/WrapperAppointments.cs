using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Appointments;
using PropertyManagerFL.Core.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Http.Headers;
using System.Text;
using System;
using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.UI.ApiWrappers;

public class WrapperAppointments : IAppointmentsService
{
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperAppointments> _logger;
    private readonly string? _appointmentsUri;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public WrapperAppointments(IConfiguration env, ILogger<WrapperAppointments> logger, HttpClient httpClient, IMapper mapper)
    {
        _env = env;
        _logger = logger;
        _appointmentsUri = $"{_env["BaseUrl"]}/Appointments";

        _httpClient = httpClient;
        _mapper = mapper;
    }

    public async Task<int> InsertAsync(AppointmentVM appointment)
    {
        try
        {
            var appointmentToInsert = _mapper.Map<Appointment>(appointment);

            using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync(_appointmentsUri, appointmentToInsert))
            {
                var output = await result.Content.ReadAsStringAsync();
                var appointmentId = JsonConvert.DeserializeObject<int>(output);

                var success = result.IsSuccessStatusCode;
                return success ? appointmentId : -1;
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"Erro ao criar marcação {exc.Message}");
            return -1;
        }
    }

    public async Task UpdateAsync(int Id, AppointmentVM appointment)
    {
        try
        {
            var updateUrl = $"{_appointmentsUri}/{Id}";
            var appointmentToUpdate = _mapper.Map<Appointment>(appointment);

            var recurrExcpt = appointmentToUpdate.RecurrenceException ?? "";
            appointmentToUpdate.RecurrenceException = recurrExcpt;
            using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync(updateUrl, appointmentToUpdate))
            {
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao atualizar marcação ({result.ReasonPhrase})");
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc);
        }
    }

    public async Task DeleteAsync(int Id)
    {
        try
        {
            await _httpClient.DeleteAsync($"{_appointmentsUri}/{Id}");
        }
        catch (HttpRequestException exc)
        {
            _logger.LogError(exc, "Erro de comunicação de rede ao apagar registro (API Appointments)");
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao apagar registo (API Appointments)");
        }
    }

    public async Task<IEnumerable<AppointmentVM>> GetAllAsync()
    {
        try
        {

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_appointmentsUri}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<AppointmentVM>>(data);
                    return output ?? Enumerable.Empty<AppointmentVM>();
                }
                else
                {
                    return Enumerable.Empty<AppointmentVM>();
                }

            }

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (Appointment)");
            return Enumerable.Empty<AppointmentVM>();
        }

    }

    public async Task<AppointmentVM> GetById_Async(int id)
    {
        try
        {
            var appointment = await _httpClient.GetFromJsonAsync<AppointmentVM>($"{_appointmentsUri}/{id}");
            var appointmentDTO = _mapper.Map<AppointmentVM>(appointment);

            return appointmentDTO ?? new();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
            return new AppointmentVM();
        }
    }

}
