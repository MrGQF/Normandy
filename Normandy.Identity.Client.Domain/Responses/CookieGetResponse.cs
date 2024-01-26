using System.Collections.Generic;

namespace Normandy.Identity.Client.Domain.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class CookieGetResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Domains { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Cookies { get; set; }
    }
}
