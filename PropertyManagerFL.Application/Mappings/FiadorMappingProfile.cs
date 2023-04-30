using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.EstadoCivil;
using PropertyManagerFL.Application.ViewModels.Fiadores;

namespace PropertyManagerFL.Application.Mappings
{
    public class FiadorMappingProfile : Profile
    {
        public FiadorMappingProfile()
        {
            CreateMap<NovoFiador, Fiador>().ReverseMap();
            CreateMap<AlteraFiador, Fiador>().ReverseMap();
            CreateMap<NovoFiador, FiadorVM>().ReverseMap();
            CreateMap<AlteraFiador, FiadorVM>().ReverseMap();
            CreateMap<Fiador, FiadorVM>().ReverseMap();

            CreateMap<EstadoCivil, ReferenciaEstadoCivil>().ReverseMap();
            CreateMap<EstadoCivil, EstadoCivilVM>().ReverseMap();
        }
    }
}
