using Newtonsoft.Json;
using Normandy.Identity.Client.Authentication.Application.Contracts;
using Normandy.Identity.Client.Authentication.Application.Contracts.Requests;
using Normandy.Identity.Client.Authentication.Application.Contracts.Responses;
using Normandy.Identity.Client.Authorization.Application.Contracts;
using Normandy.Infrastructure.Util.Crypto;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.UnitTests
{
    [TestFixture]
    public class SecurityAuthTests
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ClientManager.Instance.Init(
                new HttpCommonOptions
                {
                    AppId = "123",
                    AppKey = "123",
                    AppSecret = "123",
                    Version = "v0.1",
                    DeviceSn = "imei"
                },
                new HttpClientHandler
                {
                },
                new Configs.ConfigOptions
                {
                    Path = Directory.GetCurrentDirectory()
                });

            var instance = await ClientManager.Instance.GetAuthAsync<IAuthentication>();
            var response = await instance.SSOLogin(new SSOLoginRequest
            {
                Account = "15869330953",
                Password = "test"
            });

            Assert.That(response.Flag == 0);

        }

        [OneTimeTearDown]
        public async Task LogOut()
        {
            var instance = await ClientManager.Instance.GetAuthAsync<IAuthorization>();
            var response = await instance.LogOut();

            Console.WriteLine(JsonConvert.SerializeObject(response));
            Assert.That(response.Flag == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Login()
        {
            var instance = await ClientManager.Instance.GetAuthAsync<IAuthentication>();
            var response = await instance.SSOLogin(
                new SSOLoginRequest
                {
                    Account = "test",
                    Password = "test300033"
                });


            new Result<SSOLoginResponse>
            {
                Flag = 0,
                Code = 200,
                Message = default,
                StackTrace = default,
                Data =
                new SSOLoginResponse
                {
                    NickName = "Í«≥∆",
                    UserId = 1,
                    SessionId = "SessionId",
                    Expires = "Expires",
                    SignTime = "SignTime",
                    Sign = "thirdSign"
                }
            };


            Assert.That(response.Flag == 0);
        }

        public static Encoding GbkEncoding { get; } = Encoding.GetEncoding("GB2312");

        [Test]
        public async Task GetPassport()
        {
            var instance = await ClientManager.Instance.GetAuthAsync<IAuthorization>();
            var response = await instance.GetPassport(new Authorization.Application.Contracts.Requests.PassportRequest 
            {
                Imei = "F6968D0C25C34CC1648E83DE53E872AE",
                Newwgflag = "3",
                Nohqlist = "0",
                Product = "S01",
                QsId = "800",
                Sdsn = "",
                Securities = GbkEncoding.GetString(Encoding.UTF8.GetBytes("Õ¨ª®À≥‘∂∫Ω∞Ê")),
                Version = "8.2.1.1"
            });

            Assert.That(response.Flag == 0);
            Console.WriteLine(response.Data);
        }

        [Test]
        public async Task GetCookie()
        {
            var instance = await ClientManager.Instance.GetAuthAsync<IAuthorization>();
            var response = await instance.GetCookie(new Authorization.Application.Contracts.Requests.CookieRequest { SignValid = "20220101" });

            Console.WriteLine(JsonConvert.SerializeObject(response));
            Assert.That(response.Flag == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Encrypt()
        {
            var plainText = "test300033";
            var publicKey = "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0KTUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCZ1FEUDJ6OUM0L2lMTnJselAvUll1S05kSjNNegp2WWg3ajJ5ckdBbzlNdlpHeTRodkNKZllkalMrU1NYbisrWUVJT1JyKzlYK1NNSHU5Z0hHaTNrQnhUYkVVbVZJCmMwZWUxM3htaDEybXBjdTZRb09Hc2VEN1EyZFB4OUtqYzIxamVxSlUzV1M1UVNlbE1OM0RaVkxyd3Q5SGp5OFQKdzBDeHAweWhrQ1BaM1pWUFVRSURBUUFCCi0tLS0tRU5EIFBVQkxJQyBLRVktLS0tLQ==";
            var cryptoText = Base64Helper.Encode(RSAHelper.Encrypt(plainText, publicKey));
            Console.WriteLine(cryptoText);
        }

        [Test]
        public async Task PollCloundDataTest()
        {
            try
            {
                var instance = await ClientManager.Instance.GetAuthAsync<IAuthorization>();
                var response = await instance.DownloadSelfCodeDataAsync("", "", 1, "");

                Console.WriteLine(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }


        }
    }
}