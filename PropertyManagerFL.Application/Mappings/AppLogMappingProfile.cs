using AutoMapper;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Application.ViewModels.Security.Models;
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
