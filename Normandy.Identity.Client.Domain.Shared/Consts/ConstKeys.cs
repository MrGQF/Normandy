namespace Normandy.Identity.Client.Domain.Shared
{
    /// <summary>
    /// 常量
    /// </summary>
    public class ConstKeys
    {
        /// <summary>
        /// SDK 版本号
        /// </summary>
        public const string SDKVersion = "v0.1";

        /// <summary>
        /// 签名方式
        /// </summary>
        public const string SignMethod = "md5";

        /// <summary>
        /// 
        /// </summary>
        public const string HttpClientKey = "normandyclient";

        /// <summary>
        /// 配置文件默认名称
        /// </summary>
        public const string ConfigDefaultFileName = "normandyclientsettings.json";

        /// <summary>
        /// 配置文件默认安全V2 访问域名
        /// </summary>
        public const string ConfigDefaultSecurityDoamin = "https://security.10jqka.com.cn";

        /// <summary>
        /// 配置文件默认安全V2 访问域名 节点名
        /// </summary>
        public const string ConfigSecurityDoaminKey = "SecurityDomain";

        /// <summary>
        /// 配置文件默认安全V2 访问Host
        /// </summary>
        public const string ConfigDefaultSecurityHost = "https://security.10jqka.com.cn";

        /// <summary>
        /// 配置文件默认安全V2 访问Host 节点名
        /// </summary>
        public const string ConfigSecurityHostKey = "SecurityHost";

        /// <summary>
        /// 风控校验接口地址 节点名
        /// </summary>
        public const string RiskEventCheckUriKey = "RiskEventCheckUri";

        /// <summary>
        /// 风控校验接口默认地址
        /// </summary>
        public const string DefaultRiskEventCheckUri = "http://ultron.10jqka.com.cn/ultron/event/check";

        /// <summary>
        /// 账密登录路由
        /// </summary>
        public const string SecuritySSOLoginRoute = "/ssologin/sso/login";

        /// <summary>
        /// 安全V2 获取公钥接口路由
        /// </summary>
        public const string SecurityGetPublicKeyRoute = "/ssologin/sso/getpublickey";

        /// <summary>
        /// 安全V2 刷新令牌路由
        /// </summary>
        public const string SecurityRefreshTokenRoute = "/identify/validate/refreshtoken";

        /// <summary>
        /// 安全V2 获取通行证
        /// </summary>
        public const string SecurityGetPassportRoute = "/identify/validate/getpassport";

        /// <summary>
        /// 安全V2 获取sessionid 路由
        /// </summary>
        public const string SecurityGetSessionIdRoute = "/identify/validate/getsessionid";

        /// <summary>
        /// 安全V2 注销登录
        /// </summary>
        public const string SecurityLogoutRoute = "/identify/validate/logout";

        /// <summary>
        /// 安全V2 获取cookie
        /// </summary>
        public const string SecurityGetCookieRoute = "/identify/validate/getcookie";

        /// <summary>
        /// 配置文件默认认证中心 访问域名 
        /// </summary>
        public const string ConfigDefaultAuthCenterDomain = "";

        /// <summary>
        /// 配置文件默认认证中心 访问域名 节点名
        /// </summary>
        public const string ConfigAuthCenterDomainKey = "AuthCenterDomain";

        /// <summary>
        /// 配置文件默认认证中心 访问Host
        /// </summary>
        public const string ConfigDefaultAuthCenterHost = "";

        /// <summary>
        /// 配置文件默认认证中心 访问IP 节点名
        /// </summary>
        public const string ConfigAuthCenterHostKey = "AuthCenterHost";

        /// <summary>
        /// 定时任务调用Id-健康检查
        /// </summary>
        public const string JobSchedulerHealthCheckId = "normandyclientHealthCheck";

        /// <summary>
        /// 定时任务调用Cron-健康检查
        /// </summary>
        public const string JobSchedulerHealthCheckCron = "*/5 * * * * ?";

        /// <summary>
        /// 定时任务调用描述-健康检查
        /// </summary>
        public const string JobSchedulerHealthCheckDesc = "健康检查";

        /// <summary>
        /// 定时任务调用开始时间差-健康检查
        /// </summary>
        public const int JobSchedulerHealthCheckStartSecondLimits = 5;

        /// <summary>
        /// dns解析失败错误次数限制
        /// </summary>
        public const int DnsErrorLimit = 3;

        /// <summary>
        /// 登录失败
        /// </summary>
        public const string LoginFailed = "登录失败";

        /// <summary>
        /// 
        /// </summary>
        public const string ConfigUserAccountKey = "Account";

        /// <summary>
        /// 
        /// </summary>
        public const string ConfigUserPwdKey = "Password";
    }
}
