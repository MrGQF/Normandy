namespace Normandy.Identity.Client.Authorization.Application.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class PassportRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string QsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Sdsn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Securities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Nohqlist { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Newwgflag { get; set; }
    }
}
