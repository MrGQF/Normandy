namespace Normandy.Identity.Client.Domain.Responses
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }
}
