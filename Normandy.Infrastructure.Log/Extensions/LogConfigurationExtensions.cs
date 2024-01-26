using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Normandy.Infrastructure.Log.Extensions
{
    public static class LogConfigurationExtensions
    {
        public static LoggerConfiguration AddExcludeFilter(
            this LoggerConfiguration log,
            IConfiguration configuration,
            string Key)
        {
            log.Filter.ByExcluding(log =>
            {
                var options = new ExcludeFilterOptions();
                configuration.GetSection(Key).Bind(options);

                if (options == null
                || !options.Switch)
                {
                    return false;
                }

                if (ExcludeBySourceContext(options, log)
                || ExcludeByRequestPath(options, log))
                {
                    return true;
                }

                return false;
            });

            return log;
        }

        private static bool ExcludeBySourceContext(ExcludeFilterOptions options, LogEvent log)
        {
            if (options.SourceContext == null
                || !options.SourceContext.Any())
            {
                return false;
            }

            if (!log.Properties.TryGetValue(nameof(ExcludeFilterOptions.SourceContext), out var source))
            {
                return false;
            }

            if (options.SourceContext.Any(t => source.ToString().Contains(t)))
            {
                return true;
            }

            return false;
        }

        private static bool ExcludeByRequestPath(ExcludeFilterOptions options, LogEvent log)
        {
            if (options.RequestPath == null
                || !options.RequestPath.Any())
            {
                return false;
            }

            if (!log.Properties.TryGetValue(nameof(ExcludeFilterOptions.RequestPath), out var reqPath))
            {
                return false;
            }

            if (options.RequestPath.Any(t => reqPath.ToString().Trim('\"').ToLower() == t.ToLower()))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExcludeFilterOptions
    {
        /// <summary>
        /// 启用开关
        /// true：表示启用
        /// </summary>
        public bool Switch { get; set; }

        /// <summary>
        /// 需要过滤的来源
        /// 前缀匹配
        /// </summary>
        public List<string> SourceContext { get; set; }

        public List<string> RequestPath { get; set; }
    }
}
