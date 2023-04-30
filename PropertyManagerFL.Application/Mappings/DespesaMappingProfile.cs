using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Despesas;

namespace PropertyManagerFL.Application.Mappings
{
    public class DespesaMappingProfile : Profile
    {
        public DespesaMappingProfile()
        {
            CreateMap<Despesa, DespesaVM>().ReverseMap();
            CreateMap<NovaDespesa, Despesa>().ReverseMap();
            CreateMap<NovaDespesa, DespesaVM>().ReverseMap();
            CreateMap<AlteraDespesa, Despesa>().ReverseMap();
            CreateMap<AlteraDespesa, DespesaVM>().ReverseMap();
        }
    }
}
