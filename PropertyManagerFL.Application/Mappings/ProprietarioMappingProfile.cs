using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Proprietarios;

namespace PropertyManagerFL.Application.Mappings
{
    public class ProprietarioMappingProfile : Profile
    {
        public ProprietarioMappingProfile()
        {
            CreateMap<NovoProprietario, Proprietario>().ReverseMap();
            CreateMap<AlteraProprietario, Proprietario>().ReverseMap();
            CreateMap<NovoProprietario, ProprietarioVM>().ReverseMap();
            CreateMap<AlteraProprietario, ProprietarioVM>().ReverseMap();
            CreateMap<Proprietario, ProprietarioVM>().ReverseMap();

        }
    }
}
