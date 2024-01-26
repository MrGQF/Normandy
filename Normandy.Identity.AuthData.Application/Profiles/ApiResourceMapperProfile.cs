using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Normandy.Identity.AuthDataRpc;

namespace Normandy.Identity.AuthData.Application.Profiles
{
    public class ApiResourceMapperProfile : Profile
    {
        public ApiResourceMapperProfile()
        {
            CreateMap<IdentityServer4.Models.ApiResource, ApiResource>();

            CreateMap<IdentityServer4.Models.Secret, Secret>()
                .ConvertUsing<SecretConverter>();
        }

        public class SecretConverter : ITypeConverter<IdentityServer4.Models.Secret, Secret>
        {
            public Secret Convert(IdentityServer4.Models.Secret source, Secret destination, ResolutionContext context)
            {
                var secret = new Secret();
                secret.Type = source.Type;
                secret.Description = source.Description;
                secret.Value = source.Value;

                secret.Expiration = Timestamp.FromDateTimeOffset(source.Expiration ?? System.DateTimeOffset.MaxValue);
                return secret;
            }
        }
    }
}
