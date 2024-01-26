using System;

namespace Normandy.Infrastructure.Cache.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public static class RedisExtensions
    {
        private const string IpKey = "ARSENAL_SVC_SECURITYMS_HXREDIS_HXREDIS_TCP_IP";
        private const string PortKey = "ARSENAL_SVC_SECURITYMS_HXREDIS_HXREDIS_TCP_PORT";
        private const string HostKey = "ARSENAL_SVC_SECURITYMS_HXREDIS_HXREDIS_TCP_HOST";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetHostFromEnv()
        {
            return Environment.GetEnvironmentVariable(HostKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIpFromEnv()
        {
            return Environment.GetEnvironmentVariable(IpKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetPortFromEnv()
        {
            return Environment.GetEnvironmentVariable(PortKey);
        }
    }
}
