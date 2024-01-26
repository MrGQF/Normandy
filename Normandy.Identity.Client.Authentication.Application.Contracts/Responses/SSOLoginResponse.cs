namespace Normandy.Identity.Client.Authentication.Application.Contracts.Responses
{
    public class SSOLoginResponse
    {
        /// <summary>
        /// 账号昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// userid
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SignTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Expires { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Sign { get; set; }
    }
}
