using AutoMapper;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Mappings;
public  class HistoricoEnvioCartasMappingProfile : Profile
{
    public HistoricoEnvioCartasMappingProfile()
    {
        CreateMap<HistoricoEnvioCartas, HistoricoEnvioCartasVM>().ReverseMap();
    }
}
