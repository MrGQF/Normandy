using System;

namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    public class UserInfoNullException : NormandyIdentityClientException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UserInfoNullException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
