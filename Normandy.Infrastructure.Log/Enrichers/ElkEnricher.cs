using Serilog.Core;
using Serilog.Events;
using System;

namespace Normandy.Infrastructure.Log.Enrichers
{
    public class ElkEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("logtime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
        }
    }
}
