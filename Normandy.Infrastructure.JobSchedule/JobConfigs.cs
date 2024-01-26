using System;

namespace Normandy.Infrastructure.JobSchedule
{
    /// <summary>
    /// 任务配置
    /// </summary>
    public class JobConfigs
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Switch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 任务
        /// 必须集成自IJob
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 定时策略
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTimeOffset StartTimeUtc { get; set; }
    }
}
