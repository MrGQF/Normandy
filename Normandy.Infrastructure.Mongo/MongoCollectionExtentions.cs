using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Normandy.Infrastructure.Mongo
{
    /// <summary>
    /// 
    /// </summary>
    public static class MongoCollectionExtentions
    {
        /// <summary>
        /// 获取表的collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionFactory"></param>
        /// <param name="settings"></param>
        /// <param name="collectionNameSuffix"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IMongoCollection<T> GetCollectionEx<T>(
            this MongoDBConnectionFactory connectionFactory,
            MongoCollectionSettings settings = null,
            string collectionNameSuffix = null,
            T data = default(T),
            bool splitEnable = false)
        {
            // use Best.MongoDB split method
            if (splitEnable && string.IsNullOrWhiteSpace(collectionNameSuffix))
            {
                return connectionFactory.GetCollection(settings, data);
            }

            var type = typeof(T);
            var attr = type.GetTypeInfo().GetCustomAttribute<MongoDBCollectionNameAttribute>();
            string collectionName = splitEnable && !string.IsNullOrWhiteSpace(collectionNameSuffix)
                                    ? $"{attr?.CollectionName ?? type.Name}_{collectionNameSuffix}"
                                    : attr?.CollectionName ?? type.Name;

            // use MongoDBCollectionName or type.Name
            return connectionFactory.MongoDatabase.GetCollection<T>(collectionName, settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TIdField"></typeparam>
        /// <typeparam name="TIncreaseField"></typeparam>
        /// <param name="collection"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="increaseField"></param>
        /// <param name="increaseValue"></param>
        /// <returns></returns>
        public static Task<UpdateResult> IncreaseManyAsync<TDocument, TIdField, TIncreaseField>(
            this IMongoCollection<TDocument> collection,
            Expression<Func<TDocument, TIdField>> idField,
            TIdField idValue,
            Expression<Func<TDocument, TIncreaseField>> increaseField,
            TIncreaseField increaseValue)
        {
            var ret = collection.UpdateManyAsync(
                Builders<TDocument>.Filter.Eq(idField, idValue),
                Builders<TDocument>.Update.Inc(increaseField, increaseValue));
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TIdField"></typeparam>
        /// <param name="collection"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="increaseField"></param>
        /// <param name="increaseValue"></param>
        /// <returns></returns>
        public static Task<UpdateResult> IncreaseManyAsync<TDocument, TIdField>(
            this IMongoCollection<TDocument> collection,
            Expression<Func<TDocument, TIdField>> idField,
            TIdField idValue,
            Expression<Func<TDocument, int>> increaseField,
            int increaseValue)
        {
            return collection.IncreaseManyAsync<TDocument, TIdField, int>(idField, idValue, increaseField, increaseValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TIdField"></typeparam>
        /// <param name="collection"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="increaseField"></param>
        /// <param name="increaseValue"></param>
        /// <returns></returns>
        public static Task<UpdateResult> IncreaseManyAsync<TDocument, TIdField>(
            this IMongoCollection<TDocument> collection,
            Expression<Func<TDocument, TIdField>> idField,
            TIdField idValue,
            Expression<Func<TDocument, long>> increaseField,
            long increaseValue)
        {
            return collection.IncreaseManyAsync<TDocument, TIdField, long>(idField, idValue, increaseField, increaseValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TIdField"></typeparam>
        /// <typeparam name="TIncreaseField"></typeparam>
        /// <param name="collection"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="increaseField"></param>
        /// <param name="increaseValue"></param>
        /// <returns></returns>
        public static UpdateResult IncreaseMany<TDocument, TIdField, TIncreaseField>(
            this IMongoCollection<TDocument> collection,
            Expression<Func<TDocument, TIdField>> idField,
            TIdField idValue,
            Expression<Func<TDocument, TIncreaseField>> increaseField,
            TIncreaseField increaseValue)
        {
            var ret = collection.UpdateMany(
                Builders<TDocument>.Filter.Eq(idField, idValue),
                Builders<TDocument>.Update.Inc(increaseField, increaseValue));
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TIdField"></typeparam>
        /// <param name="collection"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="increaseField"></param>
        /// <param name="increaseValue"></param>
        /// <returns></returns>
        public static UpdateResult IncreaseMany<TDocument, TIdField>(
            this IMongoCollection<TDocument> collection,
            Expression<Func<TDocument, TIdField>> idField,
            TIdField idValue,
            Expression<Func<TDocument, int>> increaseField,
            int increaseValue)
        {
            return collection.IncreaseMany<TDocument, TIdField, int>(idField, idValue, increaseField, increaseValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TIdField"></typeparam>
        /// <param name="collection"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="increaseField"></param>
        /// <param name="increaseValue"></param>
        /// <returns></returns>
        public static UpdateResult IncreaseMany<TDocument, TIdField>(
            this IMongoCollection<TDocument> collection,
            Expression<Func<TDocument, TIdField>> idField,
            TIdField idValue,
            Expression<Func<TDocument, long>> increaseField,
            long increaseValue)
        {
            return collection.IncreaseMany<TDocument, TIdField, long>(idField, idValue, increaseField, increaseValue);
        }
    }
}
