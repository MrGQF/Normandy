using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Normandy.Identity.Client.Domain.Requests
{
    /// <summary>
    /// 风控校验请求值
    /// </summary>
    public class RiskEventCheckRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("eventModelId")]
        public string EventModelId { get; set; }

        [Description("sync")]
        public bool Sync { get; set; }

        [Description("eventData")]
        public EventData EventData { get; set; }

        public override string ToString()
        {
            var val = new RiskEventCheckRequest
            {
                EventModelId = EventModelId,
                Sync = Sync,
                EventData = EventData
            };

            return JsonSerializer.Serialize(val);
        }

    }

    public class EventData
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("userid")]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("account")]
        public string Account { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("cip")]
        public string Cip { get; set; }

        /// <summary>
        /// 设备指纹
        /// </summary>
        [JsonPropertyName("did")]
        public string Did { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("login_time")]
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 业务场景
        /// 登录：login
        /// </summary>
        [JsonPropertyName("scene")]
        public string Scene { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("tel")]
        public string Tel { get; set; }

        /// <summary>
        /// http请求状态码
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// 安全V2状态码
        /// </summary>
        [JsonPropertyName("error_type")]
        public string ErrorType { get; set; }  

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("appkey")]
        public string AppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("appver")]
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("reqtype")]
        public string ReqType { get; set; }

        /// <summary>
        /// 记录id
        /// </summary>
        [JsonPropertyName("trace_id")]
        public string TraceId { get; set; }

        /// <summary>
        /// 登录类型
        /// 3 账号密码登录
        /// </summary>
        [JsonPropertyName("logintype")]
        public string LoginType { get; set; }

        /// <summary>
        /// 终端类型
        /// </summary>
        [JsonPropertyName("ttype")]
        public string CType { get; set; }
    }
}
