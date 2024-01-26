using System.Text.Json.Serialization;

namespace Normandy.Identity.Client.Domain.Requests
{
    public class PassportGetRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("qsId")]
        public string QsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("product")]
        public string Product { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("imei")]
        public string Imei { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("sdsn")]
        public string Sdsn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("securities")]
        public string Securities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("nohqlist")]
        public string Nohqlist { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("newwgflag")]
        public string Newwgflag { get; set; }
    }
}
