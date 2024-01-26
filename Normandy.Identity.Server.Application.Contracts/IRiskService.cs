using Microsoft.AspNetCore.Http;
using Normandy.Identity.Server.Application.Contracts.Responses;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Contracts
{
    public interface IRiskService<T>
    {
        /// <summary>
        /// 事前事件：处置
        /// </summary>
        public Task BeforeHandler(HttpContext context, in RiskResult<T> result, string reqBody);

        /// <summary>
        /// 事中事件：处置
        /// </summary>
        public Task InnerHandler(HttpContext context, in RiskResult<T> result, string resBody);

        /// <summary>
        /// 事后事件：结果上传
        /// </summary>
        public Task AfterHandler(HttpContext context, in RiskResult<T> result);
    }
}
