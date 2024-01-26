using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Normandy.Infrastructure.Util.Common
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            return string.Join("&", parameters.Select(kvp =>
               string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseQueryString(this string queryString)
        {
            var reqBodyDic = HttpUtility.ParseQueryString(queryString);
            return reqBodyDic.AllKeys.Aggregate(new Dictionary<string, string>(), (des, key) =>
             {
                 var val = reqBodyDic.Get(key);
                 des.Add(key, val);
                 return des;
             });
        }
    }
}
