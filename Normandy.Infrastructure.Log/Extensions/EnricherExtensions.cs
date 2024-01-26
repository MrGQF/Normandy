using Normandy.Infrastructure.Log.Enrichers;
using Serilog;
using Serilog.Configuration;
using System;

namespace Normandy.Infrastructure.Log.Extensions
{
    public static class EnricherExtensions
    {
        /// <summary>
        /// 注意Json格式化使用：Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact
        /// </summary>
        /// <param name="enrich"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration WithElk(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<ElkEnricher>();
        }
    }
}
