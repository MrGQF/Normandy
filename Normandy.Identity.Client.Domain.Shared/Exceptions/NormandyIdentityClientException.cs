using System;

namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    /// <summary>
    /// SDK 基础异常
    /// </summary>
    public class NormandyIdentityClientException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public NormandyIdentityClientException() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NormandyIdentityClientException(string message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NormandyIdentityClientException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
