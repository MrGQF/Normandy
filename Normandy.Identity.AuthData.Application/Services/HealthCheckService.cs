using IdentityServer4.Stores;
using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Identity.AuthData.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class HealthCheckService : Health.HealthBase
    {
        private readonly IClientStore clientStore;
        private readonly IResourceStore resourceStore;
        private readonly ILogger<HealthCheckService> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientStore"></param>
        public HealthCheckService(
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<HealthCheckService> logger)
        {
            this.clientStore = clientStore;
            this.resourceStore = resourceStore;
            this.logger = logger;
        }

        public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            var client = await clientStore.FindClientByIdAsync("client");
            var scopes = await resourceStore.FindApiScopesByNameAsync(new List<string> { "apiscope" });
            if(client != null 
                && scopes != null
                && scopes.Any())
            {
                return new HealthCheckResponse() { Status = HealthCheckResponse.Types.ServingStatus.Serving };
            }

            logger.LogError("AuthData HealCheck Failed");
            throw new RpcException(Status.DefaultCancelled);                  
        }

        public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
        {
            await responseStream.WriteAsync(new HealthCheckResponse()
            { Status = HealthCheckResponse.Types.ServingStatus.Serving });
        }
    }
}
