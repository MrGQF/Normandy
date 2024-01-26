namespace Normandy.Identity.Client.Authorization.Application.Contracts.Requests
{
    public class CookieRequest
    {
        /// <summary>
        /// cookie过期时间, 格式：yyyymmdd
        /// </summary>
        public string SignValid { get; set; }
    }
}
