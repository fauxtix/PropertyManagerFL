using PropertyManagerFL.Application.ViewModels.Appointments;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface IAppointmentsService
{
    Task<int> InsertAsync(AppointmentVM appointment);
    Task<bool> UpdateAsync(int Id, AppointmentVM appointment);
    Task DeleteAsync(int id);
    Task<AppointmentVM> GetById_Async(int id);
    Task<IEnumerable<AppointmentVM>> GetAllAsync();

}
