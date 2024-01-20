using AutoMapper;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Mappings;
public class ApplicationSettingsMappingProfile : Profile
{
    public ApplicationSettingsMappingProfile()
    {
        CreateMap<ApplicationSettings, ApplicationSettingsVM>().ReverseMap();
    }
}
