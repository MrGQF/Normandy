using Normandy.Identity.Identity.Domain.Shared;
using System;

namespace Normandy.Identity.UserInfo.Application.Contracts.Dtos.Requests
{
    /// <summary>
    /// 链路追踪信息
    /// </summary>
    public class TrackInfo
    {
        /// <summary>
        /// 链路标识
        /// 记录再HttpContext.HttpRequest的 头中：x-correlation-id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 认证系统标识
        /// 0: Security
        /// 1: AuthCenter
        /// </summary>
        public AuthType AuthType { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }               

        /// <summary>
        /// 耗时
        /// </summary>
        public long TimeCostTicks { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }
    }
}
