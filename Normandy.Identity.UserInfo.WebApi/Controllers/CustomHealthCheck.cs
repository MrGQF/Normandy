using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace Normandy.Identity.UserInfo.WebApi.Controllers
{
    public class CustomHealthCheck : IHealthCheck
    {
        private readonly UserDataRpc.UserDataRpc.UserDataRpcClient client;

        public CustomHealthCheck(UserDataRpc.UserDataRpc.UserDataRpcClient client)
        {
            this.client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // 检查 UserData
            await CheckUserDataRpcServer();
            return HealthCheckResult.Healthy();
        }

        public async Task CheckUserDataRpcServer()
        {
            var info = await client.GetUserInfoAsync(new UserDataRpc.UserInfoRequest { Account = "test" });
        }
    }
}
