using AutoMapper;
using Normandy.Identity.AuthDataRpc;

namespace Normandy.Identity.AuthData.Application.Profiles
{
    public class ResourcesMapperProfile : Profile
    {
        public ResourcesMapperProfile()
        {
            CreateMap<IdentityServer4.Models.Resources, Resources>();

            CreateMap<IdentityServer4.Models.ApiScope, ApiScope>();

            CreateMap<IdentityServer4.Models.IdentityResource, IdentityResource>();
        }
    }
}
