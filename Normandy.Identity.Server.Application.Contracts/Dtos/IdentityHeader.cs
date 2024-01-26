using Normandy.Identity.Domain.Shared.Consts;
using System.Text.Json.Serialization;

namespace Normandy.Identity.Server.Application.Contracts.Dtos
{
    /// <summary>
    /// 认证头参数
    /// </summary>
    public class IdentityHeader
    {
        /// <summary>
        /// Appid
        /// </summary>
        /// 描述必填，签名需要，头中的以：“h-xxx”格式
        [JsonPropertyName(Consts.IdentityHeaderKey_AppId)]
        public string AppId { get; set; }

        /// <summary>
        /// Appkey
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_AppKey)]
        public string AppKey { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_AppSecret)]
        public string AppSecret { get; set; }

        /// <summary>
        /// 应用类型
        /// PC、IOS
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_AppType)]
        public string AppType { get; set; }

        /// <summary>
        /// 应用签名
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_AppSign)]
        public string AppSign { get; set; }

        /// <summary>
        /// 客户端版本号
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_AppVersion)]
        public string AppVersion { get; set; }

        /// <summary>
        /// 设备序列号
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_DeviceSn)]
        public string DeviceSn { get; set; }

        /// <summary>
        /// sdk版本号
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_SDKVersion)]
        public string SDKVersion { get; set; }

        /// <summary>
        /// 链路记录Id
        /// </summary>
        [JsonPropertyName(Consts.IdentityHeaderKey_TraceId)]
        public string TraceId { get; set; }
    }
}
