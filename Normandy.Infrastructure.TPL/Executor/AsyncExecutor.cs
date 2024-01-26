using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Normandy.Infrastructure.TPL.Executor
{
    /// <summary>
    /// Provides an asynchronous executor with error handling and monitor that invokes a provided execution for every item received.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncExecutor<T>
    {
        /// <summary>
        /// Error handling event
        /// </summary>
        public event EventHandler<ExecutionError<T>> OnError;

        private readonly AsyncExecutorOptions<T> _asyncExecutorOptions;
        private DateTime? _shutdownEndTime;
        private bool IsShutdown => _shutdownEndTime != null;

        private readonly ActionBlock<ExecutionInput<T>> _executionActionBlock;
        private readonly ActionBlock<ExecutionError<T>> _errorActionBlock;

        private readonly VelocityMonitor _monitor;

        #region properties Monitor

        /// <summary>
        /// 执行队列数据量
        /// </summary>
        public int ExecutionQueueCount => _executionActionBlock.InputCount;

        /// <summary>
        /// 错误队列数据量
        /// </summary>
        public int ErrorQueueCount => _errorActionBlock.InputCount;

        /// <summary>
        /// 输入
        /// </summary>
        /// <returns></returns>
        public double InputCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Input);
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public double SuccessCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Success);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <returns></returns>
        public double ErrorCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Error);
        }

        /// <summary>
        /// 溢出
        /// </summary>
        /// <returns></returns>
        public double OverflowCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Overflow);
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <returns></returns>
        public double ExceptionCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Exception);
        }

        /// <summary>
        /// 过期
        /// </summary>
        /// <returns></returns>
        public double ExpireCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Expire);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        public double CanceledCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.Canceled);
        }

        /// <summary>
        /// 执行超时
        /// </summary>
        /// <returns></returns>
        public double ExecutionTimeoutCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.ExecutionTimeout);
        }

        /// <summary>
        /// 关闭超时
        /// </summary>
        /// <returns></returns>
        public double ShutdownTimeoutCountPerSec()
        {
            return _monitor.NextVelocity(CounterName.ShutdownTimeout);
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes the <see cref="AsyncExecutor{T}"/> with the specified <see cref="Action{T, Object, CancellationToken}"/>.
        /// </summary>
        /// <param name="execution">
        /// The execution delegate to invoke with each input item received.
        /// <para>Usage:</para>
        /// <see cref="T"/> item: input, 
        /// <see cref="object"/> state: state object passed in by Post(),
        /// <see cref="CancellationToken"/> cancellationToken: requested manually or by ExecutionTimeout option.
        /// <para>
        /// <example>
        /// This sample shows how to check cancellationToken in execution.
        /// <code>
        /// (item, state, cancellationToken) => {
        ///     for (var i = 0; i &lt; 1000; i++)
        ///     {
        ///         cancellationToken.ThrowIfCancellationRequested();  // throw a OperationCanceledException
        ///         Thread.Sleep(500);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        public AsyncExecutor(Action<T, object, CancellationToken> execution) :
            this((Delegate)execution, AsyncExecutorOptions<T>.Default)
        { }

        /// <summary>
        /// Initializes the <see cref="AsyncExecutor{T}"/> with the specified <see cref="Action{T, Object, CancellationToken}"/> and <see cref="AsyncExecutorOptions{T}"/>.
        /// </summary>
        /// <param name="execution">
        /// The execution delegate to invoke with each input item received.
        /// <para>Usage:</para>
        /// <see cref="T"/> item: input, 
        /// <see cref="object"/> state: state object passed in by Post(),
        /// <see cref="CancellationToken"/> cancellationToken: requested manually or by ExecutionTimeout option.
        /// <para>
        /// <example>
        /// This sample shows how to check cancellationToken in execution.
        /// <code>
        /// (item, state, cancellationToken) => {
        ///     for (var i = 0; i &lt; 1000; i++)
        ///     {
        ///         cancellationToken.ThrowIfCancellationRequested();  // throw a OperationCanceledException
        ///         Thread.Sleep(500);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        /// <param name="asyncExecutorOptions">The options with witch to configure this <see cref="AsyncExecutor{T}"/></param>
        public AsyncExecutor(Action<T, object, CancellationToken> execution, AsyncExecutorOptions<T> asyncExecutorOptions) :
            this((Delegate)execution, asyncExecutorOptions)
        { }

        /// <summary>
        /// Initializes the <see cref="AsyncExecutor{T}"/> with the specified <see cref="Func{T, Object, CancellationToken, Task}"/>.
        /// </summary>
        /// <param name="execution">
        /// The execution delegate to invoke with each input item received.
        /// <para>Usage:</para>
        /// <see cref="T"/> item: input, 
        /// <see cref="object"/> state: state object passed in by Post(),
        /// <see cref="CancellationToken"/> cancellationToken: requested manually or by ExecutionTimeout option.
        /// <para>
        /// <example>
        /// This sample shows how to check cancellationToken in execution.
        /// <code>
        /// async (item, state, cancellationToken) => {
        ///     for (var i = 0; i &lt; 1000; i++)
        ///         cancellationToken.ThrowIfCancellationRequested();  // throw a OperationCanceledException
        ///         await Task.Delay(500).ConfigureAwait(false);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        public AsyncExecutor(Func<T, object, CancellationToken, Task> execution) :
            this((Delegate)execution, AsyncExecutorOptions<T>.Default)
        { }

        /// <summary>
        /// Initializes the <see cref="AsyncExecutor{T}"/> with the specified <see cref="Func{T, Object, CancellationToken, Task}"/> and <see cref="AsyncExecutorOptions{T}"/>.
        /// </summary>
        /// <param name="execution">
        /// The execution delegate to invoke with each input item received.
        /// <para>Usage:</para>
        /// <see cref="T"/> item: input, 
        /// <see cref="object"/> state: state object passed in by Post(),
        /// <see cref="CancellationToken"/> cancellationToken: requested manually or by ExecutionTimeout option.
        /// <para>
        /// <example>
        /// This sample shows how to check cancellationToken in execution.
        /// <code>
        /// async (item, state, cancellationToken) => {
        ///     for (var i = 0; i &lt; 1000; i++)
        ///         cancellationToken.ThrowIfCancellationRequested();  // throw a OperationCanceledException
        ///         await Task.Delay(500).ConfigureAwait(false);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        /// <param name="asyncExecutorOptions">The options with witch to configure this <see cref="AsyncExecutor{T}"/></param>
        public AsyncExecutor(Func<T, object, CancellationToken, Task> execution, AsyncExecutorOptions<T> asyncExecutorOptions) :
            this((Delegate)execution, asyncExecutorOptions)
        { }

        private AsyncExecutor(Delegate execution, AsyncExecutorOptions<T> asyncExecutorOptions)
        {
            if (execution == null) throw new ArgumentNullException(nameof(execution));
            if (asyncExecutorOptions == null) throw new ArgumentNullException(nameof(asyncExecutorOptions));

            _asyncExecutorOptions = asyncExecutorOptions.Clone();
            var errorActionBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _asyncExecutorOptions.ErrorMaxDegreeOfParallelism,
                BoundedCapacity = _asyncExecutorOptions.ErrorBoundedCapacity,
                MaxMessagesPerTask = _asyncExecutorOptions.ExecutionMaxMessagesPerTask,
                EnsureOrdered = false,
                SingleProducerConstrained = false
            };
            _errorActionBlock =
                new ActionBlock<ExecutionError<T>>(error => HandleExecutionError(error), errorActionBlockOptions);
            var executionActionBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _asyncExecutorOptions.ExecutionMaxDegreeOfParallelism,
                BoundedCapacity = _asyncExecutorOptions.ExecutionBoundedCapacity,
                MaxMessagesPerTask = _asyncExecutorOptions.ExecutionMaxMessagesPerTask,
                EnsureOrdered = false,
                SingleProducerConstrained = false
            };

            switch (execution)
            {
                case Action<T, object, CancellationToken> syncExecution:
                    _executionActionBlock =
                        new ActionBlock<ExecutionInput<T>>(input => ProcessExecutionSync(syncExecution, input),
                            executionActionBlockOptions);
                    break;
                case Func<T, object, CancellationToken, Task> asyncExecution:
                    _executionActionBlock =
                        new ActionBlock<ExecutionInput<T>>(input => ProcessExecutionAsync(asyncExecution, input),
                            executionActionBlockOptions);
                    break;
                default:
                    throw new AsyncExecutorException("execution is of incorrect delegate type");
            }
            _monitor = new VelocityMonitor();
        }

        #endregion

        private void HandleOperationCanceledException(T data, CancellationToken timeoutToken, CancellationToken postToken, Exception exception)
        {
            if (timeoutToken.IsCancellationRequested)
            {
                PostError(ExecutionErrorCode.ExecutionTimeout, data);
            }
            else if (postToken.IsCancellationRequested)
            {
                PostError(ExecutionErrorCode.Canceled, data);
            }
            else
            {
                PostError(ExecutionErrorCode.Exception, data, exception);
            }
        }

        private void ProcessExecutionSync(Action<T, object, CancellationToken> execution, ExecutionInput<T> input)
        {
            if (!CheckInput(input))
                return;

            using (var internalCts = new CancellationTokenSource(_asyncExecutorOptions.ExecutionTimeout))
            using (var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(internalCts.Token, input.CancellationToken))
            {
                try
                {
                    execution(input.Data, input.State, linkedCts.Token);
                    if (!linkedCts.Token.IsCancellationRequested)
                        _monitor.Increase(CounterName.Success);
                }
                catch (OperationCanceledException oce)
                {
                    HandleOperationCanceledException(input.Data, internalCts.Token, input.CancellationToken, oce);
                }
                catch (Exception e)
                {
                    PostError(ExecutionErrorCode.Exception, input.Data, e);
                }
            }
        }

        private async Task ProcessExecutionAsync(Func<T, object, CancellationToken, Task> execution,
            ExecutionInput<T> input)
        {
            if (!CheckInput(input))
                return;

            using (var internalCts = new CancellationTokenSource(_asyncExecutorOptions.ExecutionTimeout))
            using (var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(internalCts.Token, input.CancellationToken))
            {
                try
                {
                    await execution(input.Data, input.State, linkedCts.Token).ConfigureAwait(false);
                    if (!linkedCts.Token.IsCancellationRequested)
                        _monitor.Increase(CounterName.Success);
                }
                catch (OperationCanceledException oce)
                {
                    HandleOperationCanceledException(input.Data, internalCts.Token, input.CancellationToken, oce);
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        HandleOperationCanceledException(input.Data, internalCts.Token, input.CancellationToken, ae);
                    }
                    else
                    {
                        PostError(ExecutionErrorCode.Exception, input.Data, ae);
                    }
                }
                catch (Exception e)
                {
                    PostError(ExecutionErrorCode.Exception, input.Data, e);
                }
            }
        }

        private bool CheckInput(ExecutionInput<T> input)
        {
            if (IsShutdown && DateTime.Now > _shutdownEndTime)
            {
                PostError(ExecutionErrorCode.ShutdownTimeout, input.Data);
                return false;
            }
            if (DateTime.Now - input.InputTime > _asyncExecutorOptions.ActualDataExpireTimeSpan)
            {
                PostError(ExecutionErrorCode.Expire, input.Data);
                return false;
            }
            return true;
        }

        private bool PostError(ExecutionErrorCode errorCode, T data, Exception exception = null)
        {
            var error = new ExecutionError<T>
            {
                ErrorCode = errorCode,
                Data = data,
                Exception = exception
            };
            return _errorActionBlock.Post(error);
        }

        private void HandleExecutionError(ExecutionError<T> error)
        {
            _monitor.Increase(CounterName.Error);
            switch (error.ErrorCode)
            {
                case ExecutionErrorCode.Exception:
                    _monitor.Increase(CounterName.Exception);
                    break;
                case ExecutionErrorCode.Overflow:
                    _monitor.Increase(CounterName.Overflow);
                    break;
                case ExecutionErrorCode.Expire:
                    _monitor.Increase(CounterName.Expire);
                    break;
                case ExecutionErrorCode.Canceled:
                    _monitor.Increase(CounterName.Canceled);
                    break;
                case ExecutionErrorCode.ExecutionTimeout:
                    _monitor.Increase(CounterName.ExecutionTimeout);
                    break;
                case ExecutionErrorCode.ShutdownTimeout:
                    _monitor.Increase(CounterName.ShutdownTimeout);
                    break;
                case ExecutionErrorCode.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnError?.Invoke(this, error);
        }

        /// <summary>
        /// Post an item to execution <see cref="ActionBlock{TInput}"/>. Return immediately without consuming.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken">Producer uses a CancellationToken to cancel the execution on this item.
        /// To cancel execution when it is processing, need cooperation of the execution delegate</param>
        /// <returns>true if the item was handled properly by the async executor; false if the item wasn't handled by the async executor so the caller should handle data lossing.</returns>
        public bool Post(T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Post(item, null, cancellationToken);
        }

        /// <summary>
        /// Post an item to execution <see cref="ActionBlock{TInput}"/>. Return immediately without consuming.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state">A state object passed to delegate</param>
        /// <param name="cancellationToken">Producer uses a CancellationToken to cancel the execution on this item.
        /// To cancel execution when it is processing, need cooperation of the execution delegate</param>
        /// <returns>true if the item was handled properly by the async executor; false if the item wasn't handled by the async executor so the caller should handle data lossing.</returns>
        public bool Post(T item, object state, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsShutdown)
                return false;
            var isInputSuccess = ExecutionQueueCount < _asyncExecutorOptions.ActualBoundedCapacityOfExecution &&
                                 _executionActionBlock.Post(new ExecutionInput<T>
                                 {
                                     Data = item,
                                     State = state,
                                     InputTime = DateTime.Now,
                                     CancellationToken = cancellationToken
                                 }) ||
                                 PostError(ExecutionErrorCode.Overflow, item);
            if (isInputSuccess)
                _monitor.Increase(CounterName.Input);
            return isInputSuccess;
        }

        /// <summary>
        /// Stop receiving input and wait indefinitely to shutdown gracefully.
        /// </summary>
        /// <returns></returns>
        public int Shutdown()
        {
            return Shutdown(AsyncExecutorOptions.Infinity);
        }

        /// <summary>
        /// Stop receiving input and shutdown gracefully.
        /// After timeout, residual items have not handled will be handled as errors of <see cref="ExecutionErrorCode.ShutdownTimeout"/>.
        /// </summary>
        /// <param name="timeout">greater than <see cref="TimeSpan.Zero"/> or equals to -1 milliseconds;
        /// -1 milliseconds to wait indefinitely.</param>
        /// <returns></returns>
        public int Shutdown(TimeSpan timeout)
        {
            if (timeout != AsyncExecutorOptions.Infinity && timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            _shutdownEndTime =
                timeout == AsyncExecutorOptions.Infinity || timeout > TimeSpan.FromMilliseconds(int.MaxValue)
                    ? DateTime.MaxValue
                    : DateTime.Now.Add(timeout);
            _executionActionBlock.Complete();
            var executionCountToWait = ExecutionQueueCount;
            _executionActionBlock.Completion.Wait();
            _errorActionBlock.Complete();
            _errorActionBlock.Completion.Wait();
            return executionCountToWait;
        }
    }
}
