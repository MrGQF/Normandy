using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Mongo
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoDBConnectionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public const string TraceSourceNameProfiler = "mongodbProfiler";

        /// <summary>
        /// 
        /// </summary>
        public const string TraceSourceNameProfilerCommand = "mongodbCommand";

        private static readonly ConcurrentDictionary<string, MongoClient> ConnStrClientMap;
        private static readonly ConcurrentDictionary<string, object> TypePropertyResolverMap;

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string ConnectionString { get; }

        /// <summary>
        /// 
        /// </summary>
        public IMongoDatabase MongoDatabase { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string DataBaseName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogger LoggerCommandStarted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogger LoggerCommandSucceeded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogger LoggerCommandFailed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogger LoggerProfiler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableProfiler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        static MongoDBConnectionFactory()
        {
            ConnStrClientMap = new ConcurrentDictionary<string, MongoClient>();
            TypePropertyResolverMap = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public MongoDBConnectionFactory(string connectionString, ILogger logger)
        {
            ConnectionString = connectionString;
            CreateClient(ConnectionString);
            LoggerCommandStarted = logger;
            LoggerCommandSucceeded = logger;
            LoggerCommandFailed = logger;
            LoggerProfiler = logger;
        }


        private void CreateClient(string connectionString)
        {
            MongoUrlBuilder builder = new MongoUrlBuilder(connectionString);
            DataBaseName = builder.DatabaseName;

            if (!ConnStrClientMap.TryGetValue(connectionString, out var client))
            {
                // not contains in dic, create it
                var config = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
                config.ClusterConfigurator = cc =>
                {
                    cc.Subscribe<CommandStartedEvent>(evt =>
                    {
                        var formatter = "Executed MongoDBCommandStarted [DatabaseNamespace='{DatabaseNamespace}', ConnectionId='{ConnectionId}', OperationId='{OperationId}', RequestId='{RequestId}', CommandName='{CommandName}']  Command: {Command}";
                        LoggerCommandStarted.LogInformation(
                            new EventId(1, "MongoDBCommandStarted"),
                            formatter,
                            evt.DatabaseNamespace.ToString(),
                            evt.ConnectionId.ToString(),
                            evt.OperationId,
                            evt.RequestId,
                            evt.CommandName,
                            evt.Command.ToJson());
                    });
                    cc.Subscribe<CommandSucceededEvent>(evt =>
                    {
                        var formatter = "Executed MongoDBCommandSucceededEvent ({Duration}ms) [ConnectionId='{ConnectionId}', OperationId='{OperationId}', RequestId='{RequestId}', CommandName='{CommandName}']  Reply: {Reply}";
                        LoggerCommandSucceeded.LogInformation(
                            new EventId(2, "MongoDBCommandSucceeded"),
                            formatter,
                            evt.Duration.TotalMilliseconds,
                            evt.ConnectionId.ToString(),
                            evt.OperationId,
                            evt.RequestId,
                            evt.CommandName,
                            evt.Reply.ToJson());
                    });
                    cc.Subscribe<CommandFailedEvent>(evt =>
                    {
                        var formatter = "Executed MongoDBCommandFailedEvent ({Duration}ms) [ConnectionId='{ConnectionId}', OperationId='{OperationId}', RequestId='{RequestId}', CommandName='{CommandName}']  Failure: {Failure} StackTrace: {StackTrace}";
                        LoggerCommandFailed.LogInformation(
                            new EventId(3, "MongoDBCommandFailed"),
                            formatter,
                            evt.Duration.TotalMilliseconds,
                            evt.ConnectionId.ToString(),
                            evt.OperationId,
                            evt.RequestId,
                            evt.CommandName,
                            evt.Failure.Message,
                            evt.Failure.StackTrace);
                    });

                    if (EnableProfiler)
                    {
                        var traceSource = new TraceSource(TraceSourceNameProfiler, SourceLevels.All);
                        traceSource.Listeners.Add(new ProfileTraceListener(LoggerProfiler));
                        cc.TraceWith(traceSource);

                        var traceCommand = new TraceSource(TraceSourceNameProfilerCommand, SourceLevels.All);
                        traceCommand.Listeners.Add(new ProfileTraceListener(LoggerProfiler));
                        cc.TraceCommandsWith(traceCommand);
                    }
                };

                client = new MongoClient(config);
                ConnStrClientMap.TryAdd(connectionString, client);
            }

            MongoDatabase = client.GetDatabase(DataBaseName);
        }

        /// <summary>
        /// Get CollectionName by <see cref="System.Type"/> or <see cref="MongoDBCollectionNameAttribute"/>'s define.
        /// CollectionName can dynamic generated from <see cref="System.DateTime"/> or instance's property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal string GetEntityCollectionName<T>(T data)
        {
            var type = typeof(T);
            var attr = type.GetTypeInfo().GetCustomAttribute<MongoDBCollectionNameAttribute>();
            string collectionName = attr?.CollectionName;

            // not set attribute or CollectionName is empty, use ClassName
            if (string.IsNullOrEmpty(collectionName))
                return type.Name;

            var builder = new StringBuilder(collectionName);
            if (!string.IsNullOrWhiteSpace(attr.SuffixDateTimeFormatter))
            {
                var time = DateTime.Now.ToString(attr.SuffixDateTimeFormatter);
                builder.Append(attr.Seperator);
                builder.Append(time);
            }

            if (!string.IsNullOrWhiteSpace(attr.SuffixPeropertyName))
            {
                var cacheKey = string.IsNullOrWhiteSpace(type.FullName) ? $"{type.Namespace}_{type.Name}" : type.FullName;
                Func<T, object> propertyResolver;
                if (TypePropertyResolverMap.TryGetValue(cacheKey, out var propertyResolverObj)
                    && propertyResolverObj is Func<T, object>)
                {
                    propertyResolver = (Func<T, object>)propertyResolverObj;
                }
                else
                {
                    var arg = Expression.Parameter(type);
                    Expression expr = Expression.Property(arg, attr.SuffixPeropertyName);
                    propertyResolver = Expression.Lambda<Func<T, object>>(expr, arg).Compile();

                    TypePropertyResolverMap.TryAdd(cacheKey, propertyResolver);
                }

                string propValue = propertyResolver(data).ToString();
                builder.Append(attr.Seperator);
                builder.Append(propValue);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(MongoCollectionSettings settings = null, T data = default(T))
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            var collectionName = GetEntityCollectionName<T>(data);
            return MongoDatabase.GetCollection<T>(
                collectionName,
                settings);
        }

        /// <summary>
        /// Run command
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="readPreference"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResult> RunCommandAsync<TResult>(
            Command<TResult> command,
            ReadPreference readPreference = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            return MongoDatabase.RunCommandAsync<TResult>(command, readPreference, cancellationToken);
        }


    }
}
