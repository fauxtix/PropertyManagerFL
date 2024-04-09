using PropertyManagerFL.Application.ViewModels.Appointments;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface IAppointmentsService
{
    Task<int> InsertAsync(AppointmentVM appointment);
    Task<bool> UpdateAsync(AppointmentVM appointment);
    Task DeleteAsync(int Id);
    Task<AppointmentVM> GetById_Async(int id);
    Task<IEnumerable<AppointmentVM>> GetAllAsync();

}
