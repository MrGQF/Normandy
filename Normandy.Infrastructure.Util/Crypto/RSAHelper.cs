using RSAExtensions;
using System.Security.Cryptography;
using System.Text;

namespace Normandy.Infrastructure.Util.Crypto
{
    /// <summary>
    /// RSA加解密
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// 公钥加密 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="publicKey">pem 格式公钥</param>
        /// <param name="isPem"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string plainText, string publicKey, bool isPem = true, RSAKeyType type = RSAKeyType.Pkcs1)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPublicKey(type, publicKey, isPem);

                return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.Pkcs1);
            }
        }

        /// <summary>
        /// 私钥解密 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="privateKey">pem 格式私钥</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] crypto, string privateKey, bool isPem = true)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs1, privateKey, isPem);

                return rsa.Decrypt(crypto, RSAEncryptionPadding.Pkcs1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="privateKey">base64 字符串</param>
        /// <param name="publicKey">base64 字符串</param>
        /// <returns></returns>
        public static RSA GetRsa(string privateKey = null, string publicKey = null)
        {
            var rsa = RSA.Create();
            if (!string.IsNullOrWhiteSpace(privateKey))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs1, Base64Helper.DecodeToString(privateKey), true);
            }
            
            if (!string.IsNullOrWhiteSpace(publicKey))
            {
                rsa.ImportPublicKey(RSAKeyType.Pkcs1, Base64Helper.DecodeToString(publicKey), true);
            }            

            return rsa;
        }
    }
}
