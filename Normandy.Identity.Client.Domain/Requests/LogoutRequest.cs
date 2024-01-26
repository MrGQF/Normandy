namespace Normandy.Identity.Client.Domain.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class LogoutRequest : AuthRequestBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
