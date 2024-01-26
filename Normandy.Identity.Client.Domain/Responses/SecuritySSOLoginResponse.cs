using Normandy.Identity.Client.Domain.Dtos;

namespace Normandy.Identity.Client.Domain.Responses
{
    /// <summary>
    /// 安全V2 账密登录请求返回值
    /// </summary>
    public class SecuritySSOLoginResponse
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 令牌信息
        /// </summary>
        public TokenInfo TokenInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionInfo SessionInfo { get; set; }
    }
}
