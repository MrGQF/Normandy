using System;
using System.Threading;

namespace Normandy.Infrastructure.TPL.Executor
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExecutionInput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime InputTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
