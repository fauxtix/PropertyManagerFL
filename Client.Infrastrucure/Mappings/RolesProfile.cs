using AutoMapper;
using PropertyManagerFL.Application.Requests.Identity;
using PropertyManagerFL.Application.Responses.Identity;

namespace PropertyManagerFL.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}