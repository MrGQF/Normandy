using Normandy.Identity.Client.Authentication.Application.Contracts.Requests;
using Normandy.Identity.Client.Authentication.Application.Contracts.Responses;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Authentication.Application.Contracts
{
    /// <summary>
    /// 认证业务
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// 账密登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 1.获取公钥
        /// 2.使用公钥加密密码
        Task<Result<SSOLoginResponse>> SSOLogin(SSOLoginRequest request);
    }
}
