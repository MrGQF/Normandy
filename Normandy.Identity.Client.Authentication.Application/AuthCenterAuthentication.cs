using Normandy.Identity.Client.Authentication.Application.Contracts;
using Normandy.Identity.Client.Authentication.Application.Contracts.Requests;
using Normandy.Identity.Client.Authentication.Application.Contracts.Responses;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Authentication.Application
{
    /// <summary>
    /// 认证中心 认证业务
    /// </summary>
    public class AuthCenterAuthentication : IAuthentication
    {
        public AuthCenterAuthentication()
        {
        }

        public Task<Result<SSOLoginResponse>> SSOLogin(SSOLoginRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
