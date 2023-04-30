using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Mappings
{
    public class TipoDespesaMappingProfile : Profile
    {
        public TipoDespesaMappingProfile()
        {
            CreateMap<TipoDespesa, TipoDespesaVM>().ReverseMap();
            CreateMap<NovoTipoDespesa, TipoDespesa>().ReverseMap();
            CreateMap<NovoTipoDespesa, TipoDespesaVM>().ReverseMap();
            CreateMap<AlteraTipoDespesa, TipoDespesa>().ReverseMap();
            CreateMap<AlteraTipoDespesa, TipoDespesaVM>().ReverseMap();

        }
    }
}
