using System;

namespace Normandy.Infrastructure.TPL.Executor
{
    /// <inheritdoc />
    public class AsyncExecutorOptions<T> : AsyncExecutorOptions
    {
        internal AsyncExecutorOptions<T> Clone()
        {
            return new AsyncExecutorOptions<T>
            {
                ExecutionMaxDegreeOfParallelism = ExecutionMaxDegreeOfParallelism,
                ErrorMaxDegreeOfParallelism = ErrorMaxDegreeOfParallelism,
                ExecutionBoundedCapacity = ExecutionBoundedCapacity,
                ErrorBoundedCapacity = ErrorBoundedCapacity,
                DataExpireTimeSpan = DataExpireTimeSpan,
                ExecutionTimeout = ExecutionTimeout,
                EnableSaveErrorToFile = EnableSaveErrorToFile,
                ErrorFilePath = ErrorFilePath,
                ErrorSerializer = ErrorSerializer,
                ExecutionMaxMessagesPerTask = ExecutionMaxMessagesPerTask
            };
        }

        /// <summary>
        /// Default options
        /// </summary>
        public static readonly AsyncExecutorOptions<T> Default = new AsyncExecutorOptions<T>();

        /// <summary>
        /// 序列化错误信息
        /// </summary>
        public Func<ExecutionError<T>, string> ErrorSerializer { get; set; }
    }

    /// <summary>
    /// Configure AsyncExecutor
    /// </summary>
    public abstract class AsyncExecutorOptions
    {
        /// <summary>
        /// A constant used to specify an unlimited quantity.
        /// </summary>
        public const int Unbounded = -1;

        /// <summary>
        /// An unlimited timeout.
        /// </summary>
        public static readonly TimeSpan Infinity = TimeSpan.FromMilliseconds(Unbounded);

        private int _executionMaxDegreeOfParallelism = Environment.ProcessorCount;
        private int _errorMaxDegreeOfParallelism = Environment.ProcessorCount;
        private int _executionBoundedCapacity = Unbounded;
        private int _errorBoundedCapacity = Unbounded;
        private TimeSpan _dataExpireTimeSpan = Infinity;
        private TimeSpan _executionTimeout = Infinity;
        private int _executionMaxMessagesPerTask = Unbounded;

        /// <summary>
        /// 执行最大并行数
        /// </summary>
        public int ExecutionMaxDegreeOfParallelism
        {
            get => _executionMaxDegreeOfParallelism;
            set
            {
                if (value < 1 && value != Unbounded)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _executionMaxDegreeOfParallelism = value;
            }
        }

        /// <summary>
        /// 错误处理最大并行数
        /// </summary>
        public int ErrorMaxDegreeOfParallelism
        {
            get => _errorMaxDegreeOfParallelism;
            set
            {
                if (value < 1 && value != Unbounded)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _errorMaxDegreeOfParallelism = value;
            }
        }

        /// <summary>
        /// 执行队列最大缓存数
        /// </summary>
        public int ExecutionBoundedCapacity
        {
            get => _executionBoundedCapacity;
            set
            {
                if (value < 1 && value != Unbounded)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _executionBoundedCapacity = value;
            }
        }

        /// <summary>
        /// 错误队列最大缓存数
        /// </summary>
        public int ErrorBoundedCapacity
        {
            get => _errorBoundedCapacity;
            set
            {
                if (value < 1 && value != Unbounded)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _errorBoundedCapacity = value;
            }
        }

        /// <summary>
        /// 数据有效期，从数据传入开始计时	
        /// </summary>
        public TimeSpan DataExpireTimeSpan
        {
            get => _dataExpireTimeSpan;
            set
            {
                if (value <= TimeSpan.Zero && value != Infinity)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _dataExpireTimeSpan = value;
            }
        }

        /// <summary>
        /// 执行超时时间。
        /// greater than 0 and less than <see cref="int.MaxValue"/> milliseconds;
        /// -1 milliseconds to wait indefinitely.
        /// </summary>
        public TimeSpan ExecutionTimeout
        {
            get => _executionTimeout;
            set
            {
                if (value <= TimeSpan.Zero && value != Infinity || value.TotalMilliseconds > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _executionTimeout = value;
            }
        }

        /// <summary>
        /// 是否将错误数据写入文件
        /// </summary>
        public bool EnableSaveErrorToFile { get; set; } = true;

        /// <summary>
        /// 错误文件存储路径
        /// </summary>
        public string ErrorFilePath { get; set; }

        /// <summary>
        /// 执行超时时间。
        /// greater than 0 and less than <see cref="int.MaxValue"/>;
        /// -1 to ulimit, if data available, will continue execute on task/syncContext.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.dataflow.dataflowblockoptions.maxmessagespertask
        /// </summary>
        public int ExecutionMaxMessagesPerTask
        {
            get => _executionMaxMessagesPerTask;
            set
            {
                if (value <= 0 && value != Unbounded)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _executionMaxMessagesPerTask = value;
            }
        }

        internal int ActualMaxDegreeOfExecutionParallelism => _executionMaxDegreeOfParallelism == Unbounded ? int.MaxValue : _executionMaxDegreeOfParallelism;

        internal int ActualMaxDegreeOfErrorParallelism => _errorMaxDegreeOfParallelism == Unbounded ? int.MaxValue : _errorMaxDegreeOfParallelism;

        /// <summary>
        /// greater than or equal to 1
        /// </summary>
        internal int ActualBoundedCapacityOfExecution => _executionBoundedCapacity == Unbounded ? int.MaxValue : _executionBoundedCapacity;

        internal int ActualBoundedCapacityOfError => _errorBoundedCapacity == Unbounded ? int.MaxValue : _errorBoundedCapacity;

        internal TimeSpan ActualDataExpireTimeSpan => _dataExpireTimeSpan == Infinity ? TimeSpan.MaxValue : _dataExpireTimeSpan;
    }
}
