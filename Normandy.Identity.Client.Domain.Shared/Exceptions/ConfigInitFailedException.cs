using System;

namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    /// <summary>
    /// 尚未初始化
    /// </summary>
    public class ConfigInitFailedException : NormandyIdentityClientException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConfigInitFailedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
