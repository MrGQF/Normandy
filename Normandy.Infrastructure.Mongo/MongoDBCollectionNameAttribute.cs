using System;

namespace Normandy.Infrastructure.Mongo
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MongoDBCollectionNameAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string CollectionName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Seperator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SuffixDateTimeFormatter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SuffixPeropertyName { get; set; }


        /// <summary>
        /// Default Seperator: _
        /// </summary>
        /// <param name="collectionName"></param>
        public MongoDBCollectionNameAttribute(string collectionName)
        {
            CollectionName = collectionName;

            Seperator = "_";
        }
    }
}
