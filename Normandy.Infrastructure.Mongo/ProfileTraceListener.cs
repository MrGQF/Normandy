using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Normandy.Infrastructure.Mongo
{
    /// <summary>
    /// 
    /// </summary>
    public class ProfileTraceListener : TraceListener
    {
        /// <summary>
        /// 
        /// </summary>
        public ILogger LoggerProfiler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerProfiler"></param>
        public ProfileTraceListener(ILogger loggerProfiler)
        {
            LoggerProfiler = loggerProfiler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            LoggerProfiler.LogTrace(new EventId(11, "MongoDBProfiler"), message);
        }
    }
}
