using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    /// <summary>
    /// 持久化 资源
    /// </summary>
    public class TestResourceStore : IResourceStore
    {
        private readonly IEnumerable<IdentityServer4.Models.IdentityResource> identityResources = new[] {
                new IdentityResources.OpenId()
            };

        private readonly IEnumerable<IdentityServer4.Models.ApiScope> apiScopes = new[] {
            new IdentityServer4.Models.ApiScope("apiscope")
        };

        private readonly IEnumerable<IdentityServer4.Models.ApiResource> apiResource = new[]
            {
                new IdentityServer4.Models.ApiResource("api", "接口资源")
                    {
                        ApiSecrets =  new List<IdentityServer4.Models.Secret>{ new IdentityServer4.Models.Secret("secrets".Sha256()) },
                        //配置token携带的claim
                        UserClaims = new List<string>
                        {
                            JwtClaimTypes.Name,
                            JwtClaimTypes.NickName,
                            JwtClaimTypes.Id,
                            JwtClaimTypes.ClientId,
                            JwtClaimTypes.Role,
                        },
                        Scopes =  new[] { "apiscope"}
                    } ,
                new IdentityServer4.Models.ApiResource("api2", "接口资源2")
                    {
                        ApiSecrets =  new List<IdentityServer4.Models.Secret>{ new IdentityServer4.Models.Secret("secrets2".Sha256()) },
                        //配置token携带的claim
                        UserClaims = new List<string>
                        {
                            JwtClaimTypes.Name,
                            JwtClaimTypes.NickName,
                            JwtClaimTypes.Email,
                            JwtClaimTypes.PhoneNumber,
                            JwtClaimTypes.Id,
                            JwtClaimTypes.ClientId,
                            JwtClaimTypes.Role,
                        },
                        Scopes =  new[] { "apiscope2"}
                    }
        };

        public Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            return Task.FromResult(apiResource.Where(t => apiResourceNames.Contains(t.Name)));
        }

        public Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(apiResource.Where(t => scopeNames.Contains(t.Name)));
        }

        public Task<IEnumerable<IdentityServer4.Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(apiScopes.Where(t => scopeNames.Contains(t.Name)));
        }

        public Task<IEnumerable<IdentityServer4.Models.IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(identityResources.Where(t => scopeNames.Contains(t.Name)));
        }

        public Task<IdentityServer4.Models.Resources> GetAllResourcesAsync()
        {
            return Task.FromResult(new IdentityServer4.Models.Resources(identityResources, apiResource, apiScopes));
        }
    }
}
