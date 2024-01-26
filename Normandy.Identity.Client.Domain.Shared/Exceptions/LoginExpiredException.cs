using System;

namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginExpiredException : NormandyIdentityClientException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public LoginExpiredException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
