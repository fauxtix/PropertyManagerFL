using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Documentos;

namespace PropertyManagerFL.Application.Mappings
{
    public class DocumentosMappingProfile : Profile
    {

        public DocumentosMappingProfile()
        {
            CreateMap<NovoDocumento, Document>().ReverseMap();
            CreateMap<NovoDocumento, DocumentoVM>().ReverseMap();
            CreateMap<AlteraDocumento, Document>().ReverseMap();
            CreateMap<AlteraDocumento, DocumentoVM>().ReverseMap();

        }
    }
}
