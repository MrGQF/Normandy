using System;

namespace Normandy.Identity.Domain.Shared.Exceptions
{
    public class NormandyIdentityException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        public NormandyIdentityException() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NormandyIdentityException(string message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NormandyIdentityException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        public NormandyIdentityException(int errorCode, string message) : base(message)
        {
            base.Data.Add(nameof(ApplicationException), errorCode);
        }
    }
}
