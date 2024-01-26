namespace Normandy.Identity.Domain.Shared.Consts
{
    /// <summary>
    /// 配置key
    /// </summary>
    public class ConfigKeys
    {
        /// <summary>
        /// RSA私钥
        /// </summary>
        public const string RsaPrivateKey = "RsaPrivateKey";

        /// <summary>
        /// RSA公钥
        /// </summary>
        public const string RsaPublicKey = "RsaPublicKey";

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public const string CacheExpireSeconds = "CacheExpireSeconds";

        /// <summary>
        /// IdServer Redis缓存配置
        /// </summary>
        public const string IdentityServerRedisOptions = "IdentityServerRedisOptions";

        /// <summary>
        /// 埋点配置
        /// </summary>
        public const string TrackInfo = "TrackInfo";
    }
}
