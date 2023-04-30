using AutoMapper;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Contactos;

namespace PropertyManagerFL.Application.Mappings
{
    public class ContactoMappingProfile : Profile
    {
        public ContactoMappingProfile()
        {
            CreateMap<NovoContacto, Contact>().ReverseMap();
            CreateMap<NovoContacto, ContactoVM>().ReverseMap();
            CreateMap<AlteraContacto, Contact>().ReverseMap();
            CreateMap<AlteraContacto, ContactoVM>().ReverseMap();
        }
    }
}
