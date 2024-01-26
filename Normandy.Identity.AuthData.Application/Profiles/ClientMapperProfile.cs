using AutoMapper;
using IdentityServer4.Models;
using Google.Protobuf.WellKnownTypes;
using Normandy.Identity.AuthDataRpc;

namespace Normandy.Identity.AuthData.Application.Profiles
{
    public class ClientMapperProfile : Profile
    {
        public ClientMapperProfile()
        {
            CreateMap<Client, ClientInfo>();

            CreateMap<IdentityServer4.Models.Secret, ClientSecret>()
                .ConvertUsing<SecretConverter>();

            CreateMap<IdentityServer4.Models.ClientClaim, AuthDataRpc.ClientClaim>();
        }

        public class SecretConverter : ITypeConverter<IdentityServer4.Models.Secret, ClientSecret>
        {
            public ClientSecret Convert(IdentityServer4.Models.Secret source, ClientSecret destination, ResolutionContext context)
            {
                var secret = new ClientSecret();
                secret.Type = source.Type;
                secret.Description = source.Description;
                secret.Value = source.Value;

                secret.Expiration = Timestamp.FromDateTimeOffset(source.Expiration ?? System.DateTimeOffset.MaxValue);
                return secret;
            }
        }
    }
}
