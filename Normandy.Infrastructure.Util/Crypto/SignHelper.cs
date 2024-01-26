using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Normandy.Infrastructure.Util.Crypto
{
    /// <summary>
    /// 签名
    /// </summary>
    public static class SignHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static string Sign(Dictionary<string, string> pairs)
        {
            var builder = new StringBuilder();

            var sortedKeys = pairs.Keys.OrderBy(t => t);
            foreach (var key in sortedKeys)
            {
                if (key == "sign"
                || key == "riskinfo.ip"
                || key == "riskinfo.ctime")
                {
                    continue;
                }

                builder.Append(key.ToLower() + "=" + pairs[key] + ";");
            }

            return Md5Helper.Entry(builder.ToString());
        }
    }
}
