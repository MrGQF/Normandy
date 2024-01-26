using System;
using System.Collections.Generic;
using System.Threading;

namespace Normandy.Infrastructure.TPL.Executor
{
    internal enum CounterName
    {
        Input,
        Success,
        Error,
        Exception,
        Overflow,
        Expire,
        Canceled,
        ExecutionTimeout,
        ShutdownTimeout
    }

    /// <summary>
    /// 
    /// </summary>
    internal class VelocityMonitor
    {
        private readonly Dictionary<CounterName, Counter> _counterMap = new Dictionary<CounterName, Counter>();

        /// <summary>
        /// 
        /// </summary>
        public VelocityMonitor()
        {
            for (var i = CounterName.Input; i <= CounterName.ShutdownTimeout; i++)
            {
                _counterMap.Add(i, new Counter());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        public void Increase(CounterName counterName)
        {
            _counterMap[counterName].Increase();
        }

        public double NextVelocity(CounterName counterName)
        {
            return _counterMap[counterName].NextPerSecValue();
        }

        /// <summary>
        /// 
        /// </summary>
        private class Counter
        {
            private long _count;
            private DateTime _startTime;

            /// <inheritdoc />
            public Counter()
            {
                _count = 0;
                _startTime = DateTime.Now;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Increase()
            {
                Interlocked.Increment(ref _count);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double NextPerSecValue()
            {
                var count = Interlocked.Exchange(ref _count, 0);
                var start = _startTime;
                var end = DateTime.Now;
                _startTime = end;
                var totalSeconds = (end - start).TotalSeconds;
                if (totalSeconds > 0)
                    return count / totalSeconds;

                return 0;
            }
        }
    }
}
