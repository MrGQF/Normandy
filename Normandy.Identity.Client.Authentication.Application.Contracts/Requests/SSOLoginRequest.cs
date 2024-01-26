using System.ComponentModel;

namespace Normandy.Identity.Client.Authentication.Application.Contracts.Requests
{
    public class SSOLoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Description("account")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Description("password")]
        public string Password { get; set; }
    }
}
