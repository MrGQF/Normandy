using System;
using System.Text.Json.Serialization;

namespace Normandy.Identity.Server.Application.Contracts.Requests
{
    public class RiskEventInfo
    {
        /// <summary>
        /// http状态码
        /// </summary>
        [JsonPropertyName("status")]
        public string StatusCode { get; set; }

        /// <summary>
        /// 登录状态码
        /// </summary>
        [JsonPropertyName("error_type")]
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("uid")]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("trace_id")]
        public string TraceId { get; set; }        

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("account")]
        public string Account { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("passwd")]
        public string Password { get; set; }

        /// <summary>
        /// 设备指纹
        /// </summary>
        [JsonPropertyName("did")]
        public string Did { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("ip")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 用户绑定手机号
        /// </summary>
        [JsonPropertyName("tel")]
        public string Tel { get; set; }

        /// <summary>
        /// 请求场景
        /// </summary>
        [JsonPropertyName("scene")]
        public string Scene { get; set; } = "login";

        [JsonPropertyName("appid")]
        public string AppId { get; set; }

        [JsonPropertyName("appver")]
        public string AppVersion { get; set; }

        [JsonPropertyName("ttype")]
        public string AppType { get; set; }

        /// <summary>
        /// 登录时间
        /// 服务器时间
        /// </summary>
        [JsonPropertyName("LoginTime")]
        public string LoginTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 风控sdk 版本
        /// </summary>
        [JsonPropertyName("sdk_ver")]
        public string SdkVersion { get; set; }
        
        /// <summary>
        /// 三方登录应用类型
        /// </summary>
        [JsonPropertyName("third_login_app")]
        public string ThirdLoginType { get; set; } = string.Empty;

        /// <summary>
        /// 是否为同花顺机器人请求
        /// </summary>
        [JsonPropertyName("securities")]
        public bool Securities { get; set; } = false;

        /// <summary>
        /// 主程序签名
        /// </summary>
        [JsonPropertyName("app_sign")]
        public string AppSign { get; set; }

        /// <summary>
        /// 登录类型
        /// </summary>
        [JsonPropertyName("logintype")]
        public int LoginType { get; set; }

        /// <summary>
        /// 处置类型
        /// </summary>
        [JsonPropertyName("sdtis")]
        public string Sdtis { get; set; }

        /// <summary>
        /// 是否游客登录
        /// </summary>
        [JsonPropertyName("is_guset")]
        public bool IsGuest { get; set; } = false;
    }
}
