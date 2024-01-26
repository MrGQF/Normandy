using System.Security.Cryptography;
using System.Text;

namespace Normandy.Infrastructure.Util.Crypto
{
    /// <summary>
    /// md5 
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        /// md5 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Entry(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sBuilder = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                foreach (byte t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// md5 加密
        /// </summary>
        /// <param name="plaint"></param>
        /// <returns></returns>
        public static string Entry(byte[] plaint)
        {
            if (plaint == null || plaint.Length == 0)
                return string.Empty;

            var sBuilder = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(plaint);
                foreach (byte t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }
    }
}
