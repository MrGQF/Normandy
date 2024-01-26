namespace Normandy.Identity.Client.Domain.Shared.Exceptions
{
    public class TokenRefreshFailedException : NormandyIdentityClientException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public TokenRefreshFailedException(string message) : base(message)
        {

        }
    }
}
