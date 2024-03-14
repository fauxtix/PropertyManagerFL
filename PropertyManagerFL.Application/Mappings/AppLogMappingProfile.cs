using AutoMapper;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Mappings
{
    public class AppLogMappingProfile : Profile
    {
        public AppLogMappingProfile()
        {
            CreateMap<AppLogDto, AppLog>().ReverseMap();
        }
    }
}
