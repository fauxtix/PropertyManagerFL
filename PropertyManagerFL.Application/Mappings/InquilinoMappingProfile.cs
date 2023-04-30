using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.EstadoCivil;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.Application.Mappings
{
	public class InquilinoMappingProfile : Profile
	{
		public InquilinoMappingProfile()
		{
            CreateMap<NovoInquilino, Inquilino>().ReverseMap();
            CreateMap<AlteraInquilino, Inquilino>().ReverseMap();
            CreateMap<NovoInquilino, InquilinoVM>().ReverseMap();
            CreateMap<AlteraInquilino, InquilinoVM>().ReverseMap();
            CreateMap<Inquilino, InquilinoVM>().ReverseMap();

            CreateMap<NovoDocumentoInquilino, DocumentoInquilinoVM>().ReverseMap();
            CreateMap<AlteraDocumentoInquilino, DocumentoInquilinoVM>().ReverseMap();

            CreateMap<CC_Inquilino, CC_InquilinoVM>().ReverseMap();

            CreateMap<EstadoCivil, ReferenciaEstadoCivil>().ReverseMap();
			CreateMap<EstadoCivil, EstadoCivilVM>().ReverseMap();
		}
	}
}
