using System.ComponentModel;

namespace Normandy.Identity.Server.Application.Contracts.Requests
{
    /// <summary>
    /// 风控引擎接入接口请求值
    /// </summary>
    public class RiskRequest<T>
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("eventModelId")]
        public int ModelId { get; set; }

        /// <summary>
        /// 是否同步执行
        /// </summary>
        [Description("sync")]
        public bool Sync { get; set; }        

        /// <summary>
        /// 
        /// </summary>
        [Description("eventData")]
        public T Data { get; set; }
    }
}
