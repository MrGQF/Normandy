using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Stores
{
    public class TestClientStore : IClientStore
    {
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientName = clientId,
                AllowedScopes = new[] { "apiscope", "openid" },
                ClientSecrets = { new IdentityServer4.Models.Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedGrantTypes = new[]
                {
                    GrantType.ClientCredentials,
                    GrantType.ResourceOwnerPassword,
                    GrantType.Implicit
                },
                AllowOfflineAccess = true,
                RequireConsent = false,
                AccessTokenType = AccessTokenType.Reference,
                AccessTokenLifetime = 7200,
                RedirectUris = new List<string> { "http://localhost:5000/" }
            };

            return Task.FromResult(client);
        }
    }
}
