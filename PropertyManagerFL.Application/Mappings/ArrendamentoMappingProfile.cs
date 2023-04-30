using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;

namespace PropertyManagerFL.Application.Mappings
{
    public class ArrendamentoMappingProfile : Profile
    {
        public ArrendamentoMappingProfile()
        {
            CreateMap<Arrendamento, ArrendamentoVM>().ReverseMap();
            CreateMap<NovoArrendamento, Arrendamento>().ReverseMap();
            CreateMap<NovoArrendamento, ArrendamentoVM>().ReverseMap();
            CreateMap<AlteraArrendamento, Arrendamento>().ReverseMap();
            CreateMap<AlteraArrendamento, ArrendamentoVM>().ReverseMap();
        }
    }
}
