using Grpc.Core;
using Grpc.Health.V1;
using System.Threading.Tasks;

namespace Normandy.Identity.UserData.Application.Services
{
    /// <summary>
    /// 健康检查
    /// </summary>
    public class HealthCheckRpcService : Health.HealthBase
    {
        private readonly UserInfoService service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public HealthCheckRpcService(UserInfoService service)
        {
            this.service = service;
        }

        public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            var userInfo = await service.GetUserInfoByAccount("test");

            return new HealthCheckResponse() { Status = HealthCheckResponse.Types.ServingStatus.Serving };
        }

        public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
        {
            await responseStream.WriteAsync(new HealthCheckResponse()
            { Status = HealthCheckResponse.Types.ServingStatus.Serving });
        }
    }
}
