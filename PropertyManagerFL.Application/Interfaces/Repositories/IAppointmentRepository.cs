using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public  interface IAppointmentRepository 
{
    Task<int> InsertAsync(Appointment appointment);
    Task<bool> UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);
    Task<Appointment> GetById_Async(int id);
    Task<IEnumerable<Appointment>> GetAllAsync();

}
