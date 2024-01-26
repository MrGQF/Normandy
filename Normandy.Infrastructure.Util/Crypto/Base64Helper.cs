using System;
using System.Text;

namespace Normandy.Infrastructure.Util.Crypto
{
    public static class Base64Helper
    {
        #region Base64位加密解密
        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string EncodeToBase64(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将字节转换成base64格式
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string Encode(byte[] content)
        {
            return Convert.ToBase64String(content);
        }

        /// <summary>
        /// 将base64格式 解码
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <param name="encoding">默认Encoding.UTF8</param>
        /// <returns></returns>
        public static string DecodeToString(string content, Encoding encoding = null)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return (encoding ?? Encoding.UTF8).GetString(bytes);
        }

        /// <summary>
        /// 将base64格式 解码
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static byte[] Decode(string content)
        {
           return Convert.FromBase64String(content);
        }

        #endregion
    }
}
