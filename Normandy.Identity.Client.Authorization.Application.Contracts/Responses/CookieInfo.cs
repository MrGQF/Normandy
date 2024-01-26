using System.Collections.Generic;

namespace Normandy.Identity.Client.Authorization.Application.Contracts.Responses
{
    public class CookieInfo
    {
        /// <summary>
        /// 域名
        /// </summary>
        public List<string> Domains { get; set; }

        /// <summary>
        /// cookie
        /// </summary>
        public List<string> Cookies { get; set; }
    }
}
