namespace Normandy.Identity.Client.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientOptions
    {
        /// <summary>
        /// 安全V2 域名
        /// </summary>
        public string SecurityDomain { get; set; }

        /// <summary>
        /// 安全V2 备用Host地址
        /// </summary>
        public string SecurityHost { get; set; }

        /// <summary>
        /// 认证中心 域名
        /// </summary>
        public string AuthCenterDomain { get; set; }

        /// <summary>
        /// 认证中心 备用IP地址
        /// </summary>
        public string AuthCenterHost { get; set; }

        /// <summary>
        /// 风控校验接口地址
        /// </summary>
        public string RiskEventCheckUri { get; set; }

        /// <summary>
        /// 用户输入的用户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户输入的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 健康检查配置
        /// </summary>
        public HealthCheckConfigs HealthCheck { get; set; }

        /// <summary>
        /// 健康检查状态
        /// </summary>
        public HealthStatusConfigs HealthStatus { get; set; }
    }
}
