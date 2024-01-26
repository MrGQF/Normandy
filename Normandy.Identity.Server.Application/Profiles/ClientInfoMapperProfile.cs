using AutoMapper;
using IdentityServer4.Models;
using Normandy.Identity.AuthDataRpc;

namespace Normandy.Identity.Server.Application.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientInfoMapperProfile : Profile
    {
        public ClientInfoMapperProfile()
        {
            CreateMap<ClientInfo, Client>();

            CreateMap<ClientSecret, IdentityServer4.Models.Secret>()
                .ConvertUsing<ClientSecretConverter>();

            CreateMap<AuthDataRpc.ClientClaim, IdentityServer4.Models.ClientClaim>();

            CreateMap<AuthDataRpc.IdentityResource, IdentityServer4.Models.IdentityResource>();
        }
    }

    public class ClientSecretConverter : ITypeConverter<ClientSecret, IdentityServer4.Models.Secret>
    {
        public IdentityServer4.Models.Secret Convert(ClientSecret source, IdentityServer4.Models.Secret destination, ResolutionContext context)
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
