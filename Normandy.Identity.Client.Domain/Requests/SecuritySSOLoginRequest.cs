namespace Normandy.Identity.Client.Domain.Requests
{
    /// <summary>
    /// 安全V2 账密登录请求值
    /// </summary>
    public class SecuritySSOLoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }
    }
}
