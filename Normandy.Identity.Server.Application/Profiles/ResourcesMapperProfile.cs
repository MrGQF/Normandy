using AutoMapper;
using Normandy.Identity.AuthDataRpc;

namespace Normandy.Identity.Sever.Application.Profiles
{
    public class ResourcesMapperProfile : Profile
    {
        public ResourcesMapperProfile()
        {
            CreateMap<Resources, IdentityServer4.Models.Resources>();

            CreateMap<ApiResource, IdentityServer4.Models.ApiResource>();

            CreateMap<Secret, IdentityServer4.Models.Secret>()
                .ConvertUsing<SecretConverter>();

            CreateMap<ApiScope, IdentityServer4.Models.ApiScope>();
        }
    }

    public class SecretConverter : ITypeConverter<Secret, IdentityServer4.Models.Secret>
    {
        public IdentityServer4.Models.Secret Convert(Secret source, IdentityServer4.Models.Secret destination, ResolutionContext context)
        {
            var secret = new IdentityServer4.Models.Secret();
            secret.Type = source.Type;
            secret.Description = source.Description;
            secret.Value = source.Value;
            secret.Expiration = source.Expiration.ToDateTime();

            return secret;
        }
    }
}
