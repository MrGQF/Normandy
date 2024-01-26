namespace Normandy.Identity.Client.Domain.Dtos
{
    /// <summary>
    /// 令牌信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 令牌
        /// </summary>
        /// 当 token 超时后，可以调用token刷新接口进行刷新，每次刷新生成新的token
        public string Token { get; set; }

        /// <summary>
        /// 过期时间,秒数
        /// </summary>
        public string ExpiresIn { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// 当 refreshtoken 失效的后，需要用户重新登录
        public string RefreshToken { get; set; }

        /// <summary>
        /// refreshToken过期时间，秒数
        /// </summary>
        public string RefreshExpireIn { get; set; }
    }
}
