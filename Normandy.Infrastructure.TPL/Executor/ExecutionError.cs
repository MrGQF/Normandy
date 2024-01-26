using System;

namespace Normandy.Infrastructure.TPL.Executor
{
    /// <summary>
    /// 错误
    /// </summary>
    public class ExecutionError<T>
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        public ExecutionErrorCode ErrorCode { get; set; }

        /// <summary>
        /// 发生错误的数据
        /// </summary>
        public T Data { get; set; }


        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ExecutionErrorCode
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown,

        /// <summary>
        /// 异常
        /// </summary>
        Exception,

        /// <summary>
        /// 溢出
        /// </summary>
        Overflow,

        /// <summary>
        /// 过期
        /// </summary>
        Expire,

        /// <summary>
        /// 生产者取消执行
        /// </summary>
        Canceled,

        /// <summary>
        /// 执行超时
        /// </summary>
        ExecutionTimeout,

        /// <summary>
        /// 关闭超时
        /// </summary>
        ShutdownTimeout
    }

    /// <summary>
    /// 
    /// </summary>
    public class AsyncExecutorException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public AsyncExecutorException()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AsyncExecutorException(string message, Exception innerException = null) : base(message, innerException)
        {

        }
    }
}
