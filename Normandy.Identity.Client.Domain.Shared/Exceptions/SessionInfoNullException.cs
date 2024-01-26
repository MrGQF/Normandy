using System;

namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    public class SessionInfoNullException : NormandyIdentityClientException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SessionInfoNullException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
