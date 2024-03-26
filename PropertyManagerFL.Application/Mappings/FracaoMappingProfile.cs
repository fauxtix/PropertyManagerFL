using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.EstadoConservacao;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.SituacaoFracao;
using PropertyManagerFL.Application.ViewModels.TipologiaFracao;
using PropertyManagerFL.Application.ViewModels.TipoPropriedade;


namespace PropertyManagerFL.Application.Mappings
{
	public class FracaoMappingProfile : Profile
	{
		public FracaoMappingProfile()
		{
            CreateMap<NovaFracao, Fracao>().ReverseMap();
            CreateMap<NovaFracao, FracaoVM>().ReverseMap();
            CreateMap<AlteraFracao, Fracao>().ReverseMap();
            CreateMap<AlteraFracao, FracaoVM>().ReverseMap();

            CreateMap<TipologiaFracao, NovaTipologiaFracao>();
			CreateMap<TipologiaFracao, ReferenciaTipologiaFracao>().ReverseMap();
			CreateMap<TipologiaFracao, TipologiaFracaoVM>().ReverseMap();

			CreateMap<TipoPropriedade, NovoTipoPropriedade>();
			CreateMap<TipoPropriedade, ReferenciaTipoPropriedade>().ReverseMap();
			CreateMap<TipologiaFracao, TipologiaFracaoVM>().ReverseMap();

			CreateMap<SituacaoFracao, NovaSituacaoFracao>();
			CreateMap<SituacaoFracao, ReferenciaSituacaoFracao>().ReverseMap();
			CreateMap<SituacaoFracao, SituacaoFracaoVM>().ReverseMap();

			CreateMap<EstadoConservacao, NovoEstadoConservacao>();
			CreateMap<EstadoConservacao, ReferenciaEstadoConservacao>().ReverseMap();
			CreateMap<EstadoConservacao, EstadoConservacaoVM>().ReverseMap();

            CreateMap<Seguro, SeguroVM>().ReverseMap();
        }
    }
}
