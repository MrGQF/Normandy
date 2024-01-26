namespace Normandy.Identity.Client.Authentication.Application.Contracts.Responses
{
    /// <summary>
    /// 返回值
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// 结果标识
        /// 0: 成功; -1:失败;
        /// </summary>
        public int Flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 错误堆栈
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
    }
}
