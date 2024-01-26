using System;

namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenInfoNullException : NormandyIdentityClientException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TokenInfoNullException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
