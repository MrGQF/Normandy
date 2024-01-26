using System.ComponentModel;

namespace Normandy.Identity.Client
{
    /// <summary>
    /// 接口通用参数
    /// </summary>
    public class HttpCommonOptions
    {
        /// <summary>
        /// Appid
        /// 客户端必传
        /// </summary>
        /// 描述必填，签名需要，头中的以：“h-xxx”格式
        [Description("h-appid")]
        public string AppId { get; set; }

        /// <summary>
        /// Appkey
        /// 客户端必传
        /// </summary>
        [Description("h-appkey")]
        public string AppKey { get; set; }

        /// <summary>
        /// AppSecret
        /// 客户端必传
        /// </summary>
        [Description("h-appsecret")]
        public string AppSecret { get; set; }

        /// <summary>
        /// 客户端版本号
        /// 客户端必传
        /// </summary>
        [Description("h-version")]
        public string Version { get; set; }
        
        /// <summary>
        /// 设备序列号
        /// 客户端必传
        /// </summary>
        [Description("h-devicesn")]
        public string DeviceSn { get; set; }

        /// <summary>
        /// sdk版本号
        /// </summary>
        [Description("h-sdkversion")]
        public string SDKVersion { get; set; }

        /// <summary>
        /// 签名类型, 枚举类型,0:md5;1:hmac;2:hmac-sha256
        /// </summary>
        [Description("h-signmethod")]
        public string SignMethod { get; set; }
    }
}
