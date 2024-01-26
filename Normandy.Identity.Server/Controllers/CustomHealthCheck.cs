using Microsoft.Extensions.Diagnostics.HealthChecks;
using Normandy.Identity.AuthDataRpc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Controllers
{
    public class CustomHealthCheck : IHealthCheck
    {
        private readonly ClientStoreRpc.ClientStoreRpcClient clientStoreClient;
        private readonly UserDataRpc.UserDataRpc.UserDataRpcClient userDataRpcClient;
        private readonly ResourceStoreRpc.ResourceStoreRpcClient resourceStoreRpcClient;

        public CustomHealthCheck(
            ClientStoreRpc.ClientStoreRpcClient clientStoreClient,
            UserDataRpc.UserDataRpc.UserDataRpcClient userDataRpcClient,
            ResourceStoreRpc.ResourceStoreRpcClient resourceStoreRpcClient)
        {
            this.clientStoreClient = clientStoreClient;
            this.userDataRpcClient = userDataRpcClient;
            this.resourceStoreRpcClient = resourceStoreRpcClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // 检查 AuthData
            var client = await clientStoreClient.FindClientByIdAsyncAsync(new ClientRequest { Id = "client" });
            if(client?.Data == null)
            {
                return HealthCheckResult.Unhealthy("cannot find client by id: client");
            }
            var resourceRequest = new ApiResourceRequest();
            resourceRequest.ApiResourceNames.Add(new List<string> { "api" });
            var apiResources = await resourceStoreRpcClient.FindApiResourcesAsyncAsync(resourceRequest);
            if(apiResources?.Data == null
                || !apiResources.Data.Any())
            {
                return HealthCheckResult.Unhealthy("cannot find ApiResources by Name: api");
            }

            // 检查 UserData
            await CheckUserDataRpcServer();
            return HealthCheckResult.Healthy();
        }

        public async Task CheckUserDataRpcServer()
        {
            _ = await userDataRpcClient.GetUserInfoAsync(new UserDataRpc.UserInfoRequest { Account = "test" });
        }
    }
}
