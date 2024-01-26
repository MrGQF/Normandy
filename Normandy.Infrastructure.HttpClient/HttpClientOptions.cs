using System.Collections.Generic;
using System.Net.Http;

namespace Normandy.Infrastructure.HttpClient
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpClientOptions
    {
        /// <summary>
        /// 必填
        /// 每个连接的唯一标识  
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 可空
        /// HttpClientHandler 不为空时,key必填，不然会找不到
        /// </summary>
        public HttpClientHandler Handler { get; set; }

        /// <summary>
        /// Http 头信息
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
    }
}
