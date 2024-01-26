using System.Text.Json.Serialization;

namespace Normandy.Identity.Server.Application.Contracts.Responses
{
    public class RiskDisposeInfo
    {
        /// <summary>
        /// 处置标识
        /// </summary>
        [JsonPropertyName("disposeId")]
        public string Id { get; set; }

        /// <summary>
        /// 处置类型id
        /// </summary>
        [JsonPropertyName("disposeMethodTypeId")]
        public int MethodTypeId { get; set; }
    }
}
