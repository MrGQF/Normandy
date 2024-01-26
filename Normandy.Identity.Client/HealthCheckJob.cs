using Quartz;
using System.Threading.Tasks;

namespace Normandy.Identity.Client
{
    /// <summary>
    /// 健康检查
    /// </summary>
    internal class HealthCheckJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
