using Microsoft.IdentityModel.Tokens;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.Crypto;
using NUnit.Framework;
using RSAExtensions;
using System;
using System.Security.Cryptography;

namespace Normandy.Identity.Client.UnitTests
{
    [TestFixture]
    public class EncryTests
    {
        private string publicKey;
        private string privateKey;

        [Test]
        public void GetKey()
        {           
            var rsa = RSA.Create(2048);
            // 导出            
            privateKey = rsa.ExportPrivateKey(RSAKeyType.Pkcs1, true);        
            publicKey = rsa.ExportPublicKey(RSAKeyType.Pkcs1, true);
            
            // 导入
            rsa.ImportPrivateKey(RSAKeyType.Pkcs1, privateKey, true); 
            rsa.ImportPublicKey(RSAKeyType.Pkcs1, publicKey, true);

            var securityKey = new RsaSecurityKey(rsa);
        }

        [Test]
        public void DecryptTest()
        {
            var privateKeyBase64 = Base64Helper.Encode(System.Text.Encoding.Default.GetBytes(privateKey));
            var publicKeyBase64 = Base64Helper.Encode(System.Text.Encoding.Default.GetBytes(publicKey));

            Console.WriteLine("RSA PKCS#1 PEM 私钥：");
            Console.WriteLine(privateKeyBase64);
            Console.WriteLine("RSA PKCS#1 PEM 公钥：");
            Console.WriteLine(publicKeyBase64);

            
            privateKey = Base64Helper.DecodeToString(privateKeyBase64);
            publicKey = Base64Helper.DecodeToString(publicKeyBase64);

            var plaint = "pwd";
            var encry = RSAHelper.Encrypt(plaint, publicKey);

            var decry = System.Text.Encoding.UTF8.GetString(RSAHelper.Decrypt(encry, privateKey));
            Assert.That(plaint, Is.EqualTo(decry));
        } 
        
        [Test]
        public void EncryTest()
        {
            var plaint = "123456";
            var publicKeyBase64 = "LS0tLS1CRUdJTiBSU0EgUFVCTElDIEtFWS0tLS0tDQpNSUlCQ2dLQ0FRRUF2OEFhRE9scFBiUkZxQkNnWnRnTXdzMWsvMlg3clc3NHdzNDI1M1UwSmNOaFNZNVl4SkE4DQpSWGRNVlBYS3I4K1loaGhkNU5tTDQwTTc2dlRMck5KTzVZNGRjb1RXUitma0ZIalhCTTVac2g1dkhKaFlrNEllDQpvWUt6NVVKR04vLzZLbGp4cFd2Z2NQQXd2UEN3QkYyT25veDRlYy9sY3lEVEdXckhNcThhZ2pDTnV1OFlzTEpEDQpwcjZjKzYyTk1SbFJBR01rS2pWb0ZMYVpHUXRRQklSSzVNUktZRTE3amJBc2ZmVkZoUlQ3MnlVVDNTc2tFY3J2DQozTm05VTZ1U2FiVDAxUFQ5NlRLSEZwN2xtZDE5QlN6T1M0MURvTjV0RGJUY1dPczA5cy93NlBzRkdUNHVUV21uDQo4aEQvZmZSYWtHVHI2NFRZREU3Q3hld2E0dWdQZWdyR1dRSURBUUFCDQotLS0tLUVORCBSU0EgUFVCTElDIEtFWS0tLS0tDQo=";
            publicKey = Base64Helper.DecodeToString(publicKeyBase64);
            var encry = RSAHelper.Encrypt(plaint, publicKey);

            Console.WriteLine(Base64Helper.Encode(encry));
        }

        [Test]
        public void SerilizeTest()
        {
            var response = new SerilizerTest { Code = 1, Message = "msg"  };
            var res = response.ToDictionary();
            Assert.That(res.TryGetValue("Code", out var val));
            Assert.That(val == "1");
        }
    }

    public class SerilizerTest
    {
        [System.ComponentModel.Description("Code")]
        public int Code { get; set; }

        [System.ComponentModel.Description("Message")]
        public string Message { get; set; }
    }
}
