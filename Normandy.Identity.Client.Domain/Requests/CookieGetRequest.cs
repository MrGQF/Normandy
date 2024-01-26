namespace Normandy.Identity.Client.Domain.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class CookieGetRequest : AuthRequestBase
    {
        /// <summary>
        /// cookie过期时间, 格式：yyyymmdd
        /// </summary>
        public string SignValid { get; set; }
    }
}
