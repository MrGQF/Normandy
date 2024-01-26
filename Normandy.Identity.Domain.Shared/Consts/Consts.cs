namespace Normandy.Identity.Domain.Shared.Consts
{
    public class Consts
    {
        /// <summary>
        /// 跨域标识
        /// </summary>
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        
        /// <summary>
        /// 健康检查路由
        /// </summary>
        public const string HealthCheckRoute = "/health";

        #region 通用头参数       
        public const string IdentityHeaderKey_AppId = "h-appid";
        public const string IdentityHeaderKey_AppKey = "h-appkey";
        public const string IdentityHeaderKey_AppSecret = "h-appsecret";
        public const string IdentityHeaderKey_AppType = "h-apptype";
        public const string IdentityHeaderKey_AppSign = "h-appsign";
        public const string IdentityHeaderKey_AppVersion = "h-version";
        public const string IdentityHeaderKey_DeviceSn = "h-devicesn";
        public const string IdentityHeaderKey_SDKVersion = "h-sdkversion";
        public const string IdentityHeaderKey_TraceId = "x-correlation-id";
        #endregion
    }
}
