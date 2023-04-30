using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.EstadoConservacao;
using PropertyManagerFL.Application.ViewModels.Imoveis;

namespace PropertyManagerFL.Application.Mappings
{
    public class ImovelMappingProfile : Profile
    {
        public ImovelMappingProfile()
        {
            CreateMap<NovoImovel, Imovel>().ReverseMap();
            CreateMap<NovoImovel, ImovelVM>().ReverseMap();
            CreateMap<AlteraImovel, Imovel>().ReverseMap();
            CreateMap<AlteraImovel, ImovelVM>().ReverseMap();

            CreateMap<EstadoConservacao, NovoEstadoConservacao>();
            CreateMap<EstadoConservacao, ReferenciaEstadoConservacao>().ReverseMap();
            CreateMap<EstadoConservacao, EstadoConservacaoVM>().ReverseMap();

        }
    }
}
