using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Normandy.Infrastructure.TPL.Async
{ 
    /// <summary>
    /// 
    /// </summary>
    public static class ParallelExtentions
    {
        /// <summary>
        /// 
        /// </summary>
        public class ParallelContextInfo
        {
            /// <summary>
            /// used in context log or other
            /// </summary>
            public string RequestGuid { get; set; }
        }

        [ThreadStatic]
        private static string _requestGuid;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestGuid"></param>
        public static void SetRequestGuid(string requestGuid)
        {
            _requestGuid = requestGuid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRequestGuid()
        {
            return _requestGuid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ParallelContextInfo GetParallelContextInfo()
        {
            var requestGuid = GetRequestGuid();

            return new ParallelContextInfo
            {
                RequestGuid = requestGuid,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public static void SetParallelContextInfoForTask(ParallelContextInfo info)
        {
            SetRequestGuid(info.RequestGuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="loopState"></param>
        /// <param name="info"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ParallelContextInfo ParallelBodyFunc<TSource>(TSource source, ParallelLoopState loopState, ParallelContextInfo info, Action<TSource> body)
        {
            SetParallelContextInfoForTask(info);

            body(source);

            return info;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            return ForEach(source, new ParallelOptions { MaxDegreeOfParallelism = 10 }, body);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="parallelOptions"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)
        {
            var info = GetParallelContextInfo();

            return Parallel.ForEach(source, parallelOptions, arg =>
            {
                // set context
                SetParallelContextInfoForTask(info);

                // real invoke
                body(arg);
            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task ForEachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> body)
        {
            await ForEachAsync(source, new ParallelOptions { MaxDegreeOfParallelism = 10 }, body).ConfigureAwait(false);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="parallelOptions"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task ForEachAsync<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Func<TSource, Task> body)
        {
            var info = GetParallelContextInfo();
            var tfBlock = new ActionBlock<TSource>(async data =>
            {
                SetParallelContextInfoForTask(info);
                await body(data).ConfigureAwait(false);
            },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism,
                    CancellationToken = parallelOptions.CancellationToken,
                    TaskScheduler = parallelOptions.TaskScheduler
                });
            foreach (var s in source)
                tfBlock.Post(s);
            tfBlock.Complete();

            await tfBlock.Completion.ConfigureAwait(false);
        }

        /// <summary>
        /// <see cref="Parallel.Invoke(System.Action[])"/>
        /// </summary>
        /// <param name="actions"></param>
        public static void Invoke(params Action[] actions)
        {
            var info = GetParallelContextInfo();

            var contextActions = actions.Select(act =>
            {
                Action proxyAction = () =>
                {
                    SetParallelContextInfoForTask(info);
                    act();
                };
                return proxyAction;
            }).ToArray();

            Parallel.Invoke(contextActions);
        }

        /// <summary>
        /// <see cref="Parallel.Invoke(System.Action[])"/>
        /// </summary>
        /// <param name="actions"></param>
        public static async Task InvokeAsync(params Func<Task>[] actions)
        {
            var info = GetParallelContextInfo();

            var tasks = new ConcurrentBag<Task>();
            foreach (var act in actions)
            {
                // set context
                SetParallelContextInfoForTask(info);
                // real invoke
                var task = act();
                tasks.Add(task);
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
