namespace Normandy.Identity.Client.Configs
{
    public class HealthCheckConfigs
    {
        /// <summary>
        /// 开关
        /// </summary>
        public bool Switch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 定时策略
        /// </summary>
        public string Corn { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 开始时间与当前时间秒数差
        /// </summary>
        public int StartSecondLimits { get; set; }
    }
}
