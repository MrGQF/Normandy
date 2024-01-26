using Microsoft.Extensions.Logging;
using System;

namespace Normandy.Infrastructure.Log.Provider
{
    public class EFCoreLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new EFCoreLogger(categoryName);
        public void Dispose() { }
    }

    public class EFCoreLogger : ILogger
    {
        private readonly string categoryName;

        public EFCoreLogger(string categoryName) => this.categoryName = categoryName;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (categoryName == "Microsoft.EntityFrameworkCore.Database.Command")
            {
                var logContent = formatter(state, exception);
                
                Serilog.Log.Debug("【SQL语句】：" + logContent);
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
