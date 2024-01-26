using Normandy.Identity.Server.Application.Contracts.Requests;
using System.Collections.Generic;

namespace Normandy.Identity.Server.Application.Contracts.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class RiskResult<T>
    {
        /// <summary>
        /// 是否处置
        /// True： 处置
        /// false: 不处置
        /// </summary>
        public bool Disposed { get; set; }

        /// <summary>
        /// 处置信息
        /// </summary>
        public IList<RiskDisposeInfo> DisposeInfo { get; set; }

        /// <summary>
        /// 请求值
        /// </summary>
        public RiskRequest<T> Request { get; set; }
    }
}
