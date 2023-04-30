using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.Application.Mappings
{
    public class RecebimentoMappingProfile : Profile
    {
        public RecebimentoMappingProfile()
        {
            CreateMap<Recebimento, RecebimentoVM>().ReverseMap();
            CreateMap<NovoRecebimento, Recebimento>().ReverseMap();
            CreateMap<NovoRecebimento, RecebimentoVM>().ReverseMap();
            CreateMap<AlteraRecebimento, Recebimento>().ReverseMap();
            CreateMap<AlteraRecebimento, RecebimentoVM>().ReverseMap();

        }
    }
}
