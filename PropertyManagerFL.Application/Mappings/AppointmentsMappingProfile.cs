using AutoMapper;
using PropertyManagerFL.Application.ViewModels.Appointments;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Mappings;
public  class AppointmentsMappingProfile : Profile
{
    public AppointmentsMappingProfile()
    {
        CreateMap<AppointmentVM, Appointment>().ReverseMap();

    }
}
