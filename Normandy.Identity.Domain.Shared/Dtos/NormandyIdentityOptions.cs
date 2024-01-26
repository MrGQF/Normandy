using System.Collections.Generic;

namespace Normandy.Identity.Domain.Shared.Dtos
{
    public class NormandyIdentityOptions
    {
        public int HttpTimeOutSeconds { get; set; }
        
        /// <summary>
        /// 是否启用grpc api
        /// </summary>
        public bool IsUseGrpcHttpApi { get; set; }

        public string UserDataGrpcAddress { get; set; }

        public string AuthDataGrpcAddress { get; set; }

        #region AuthDataRpc
        public int AuthDataRpcServerHttp2Port { get; set; }
        public int AuthDataRpcServerHttp1Port { get; set; }
        public IList<string> AuthDbConnectionstrings { get; set; }
        #endregion

        #region UserDataRpc
        public int UserDataRpcServerHttp2Port { get; set; }
        public int UserDataRpcServerHttp1Port { get; set; }
        public IList<string> UserDbConnectionstrings { get; set; }

        /// <summary>
        /// 认证中心Mkr 服务地址
        /// </summary>
        public string AuthMkrHost { get; set; }

        /// <summary>
        /// 认证中心Verify 服务地址
        /// </summary>
        public string AuthVerifyHost { get; set; }

        /// <summary>
        /// 认证中心 密码系统地址
        /// </summary>
        public string AuthPwdSystemHost { get; set; }
        #endregion

        #region UserInfoWebApi
        public string[] UserInfoWebApiServerUrls { get; set; }
        public string AuthorityAddress { get; set; }
        public string UserInfoApiName { get; set; }
        public string UserInfoApiSecret { get; set; }
        #endregion

        #region IdentityServer
        public bool UseHxRedis { get; set; } = true;
        public string[] IdentityServerUrls { get; set; }
        public int PasswordLength { get; set; }
        public int TokenHandleLength { get; set; }
        public string CerName { get; set; }
        public string CerPwd { get; set; }
        public string RedisConnectionString { get; set; }

        /// <summary>
        /// 风控-开关
        /// </summary>
        public bool RiskSwitch { get; set; }

        /// <summary>
        /// 风控-超时时间
        /// </summary>
        public int RiskHttpTimeOutMilliseconds { get; set; }

        /// <summary>
        /// 风控-校验接口地址
        /// </summary>
        public string RiskCheckUrl { get; set; }
        #endregion
    }
}
