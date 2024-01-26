using AutoMapper;
using IdentityServer4.Stores;
using Grpc.Core;
using Normandy.Identity.AuthDataRpc;
using Normandy.Identity.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Identity.AuthData.Application.Services
{
    public class ResourceStoreRpcService : ResourceStoreRpc.ResourceStoreRpcBase
    {
        private readonly IResourceStore store;
        private readonly IMapper mapper;

        public ResourceStoreRpcService(IResourceStore store, IMapper mapper)
        {
            this.store = store;
            this.mapper = mapper;
        }

        public override async Task<ApiResourceResult> FindApiResourcesAsync(ApiResourceRequest request, ServerCallContext context)
        {
            var result = new ApiResourceResult();

            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                IEnumerable<IdentityServer4.Models.ApiResource>? resources = default;
                if (request.ScopeNames?.Count > 0)
                {
                    resources = await store.FindApiResourcesByScopeNameAsync(request.ScopeNames);
                    if (resources == null || resources.Count() <= 0)
                    {
                        result.Code = (int)NormandyIdentityErrorCodes.ApiResourcesByScopeNameNotFound;
                        result.Message = NormandyIdentityErrorCodes.ApiResourcesByScopeNameNotFound.ToString();
                        return result;
                    }
                }

                if (request.ApiResourceNames?.Count > 0)
                {
                    resources = await store.FindApiResourcesByNameAsync(request.ApiResourceNames);
                }

                if (resources == null || resources.Count() <= 0)
                {
                    result.Code = (int)NormandyIdentityErrorCodes.ApiResourcesByNameNotFound;
                    result.Message = NormandyIdentityErrorCodes.ApiResourcesByNameNotFound.ToString();
                    return result;
                }

                var data = mapper.Map<IEnumerable<IdentityServer4.Models.ApiResource>, IEnumerable<ApiResource>>(resources);
                result.Data.AddRange(data);
            }
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.ApiResourcesFindFailed;
                result.Message = $"{ex.Message}, {ex.StackTrace}";
            }

            return result;
        }

        public override async Task<ApiScopeResult> FindApiScopesByNameAsync(ApiScopeRequest request, ServerCallContext context)
        {
            var result = new ApiScopeResult();
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var scopes = await store.FindApiScopesByNameAsync(request.ScopeNames);
                if (scopes == null || scopes.Count() <= 0)
                {
                    result.Code = (int)NormandyIdentityErrorCodes.ApiScopesByNameNotFound;
                    result.Message = NormandyIdentityErrorCodes.ApiScopesByNameNotFound.ToString();
                    return result;
                }

                var data = mapper.Map<IEnumerable<IdentityServer4.Models.ApiScope>, IEnumerable<ApiScope>>(scopes);
                result.Data.AddRange(data);
            }           
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.ApiScopesByNameFindFailed;
                result.Message = $"{ex.Message}, {ex.StackTrace}";
            }

            return result;
        }

        public override async Task<IdentityResourceResult> FindIdentityResourcesByScopeNameAsync(IdentityResourceRequest request, ServerCallContext context)
        {
            var result = new IdentityResourceResult();
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var resources = await store.FindIdentityResourcesByScopeNameAsync(request.ScopeNames);
                if (resources == null || resources.Count() <= 0)
                {
                    result.Code = (int)NormandyIdentityErrorCodes.IdentityResourcesByScopeNameNotFound;
                    result.Message = NormandyIdentityErrorCodes.IdentityResourcesByScopeNameNotFound.ToString();
                    return result;
                }

                var data = mapper.Map<IEnumerable<IdentityServer4.Models.IdentityResource>, IEnumerable<IdentityResource>>(resources);
                result.Data.AddRange(data);
            }
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.IdentityResourcesByScopeNameFindFailed;
                result.Message = $"{ex.Message}, {ex.StackTrace}";
            }

            return result;
        }

        public override async Task<ResourcesResult> GetAllResourcesAsync(ResourcesRequest request, ServerCallContext context)
        {
            var result = new ResourcesResult();
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var resources = await store.GetAllResourcesAsync();
                if (resources == null)
                {
                    result.Code = (int)NormandyIdentityErrorCodes.AllResourcesNotFound;
                    result.Message = NormandyIdentityErrorCodes.AllResourcesNotFound.ToString();
                    return result;
                }

                var data = mapper.Map<IdentityServer4.Models.Resources, Resources>(resources);
                result.Data.Add(data);
            }
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AllResourcesFindFailed;
                result.Message = $"{ex.Message}, {ex.StackTrace}";
            }

            return result;
        }
    }
}
