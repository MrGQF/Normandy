using Microsoft.Extensions.Configuration;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Domain.Shared.Exceptions;
using Normandy.Identity.Identity.Domain.Shared;
using Normandy.Infrastructure.DI;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.HttpUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Normandy.Identity.UserData.Application.Services
{
    /// <summary>
    /// 认证中心 接口服务
    /// </summary>
    public class AuthCenterService : IScopedAutoDIable
    {
        private readonly HttpClient httpClient;
        private readonly NormandyIdentityOptions config = new NormandyIdentityOptions();

        public AuthCenterService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            configuration.Bind(config);

            httpClient = clientFactory.CreateClient(AuthType.AuthCenter.ToString());
            httpClient.Timeout = TimeSpan.FromSeconds(config.HttpTimeOutSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="md5Pwd"></param>
        /// <param name="userId"></param>
        /// <param name="cIp"></param>
        /// <returns></returns>
        /// <exception cref="AuthGetPublicKeyReturnNullErrorException"></exception>
        /// <exception cref="AuthPwdNotValidException"></exception>
        public async Task AuthCheckPassword(
            string md5Pwd,
            int userId,
            string cIp)
        {                     
            var req = new Dictionary<string, string>()
            {
                { "userid", userId.ToString() },
                { "passwd", md5Pwd},
                { "cip", cIp },
                {"app_flag", "0Z000001" },
                {"reqfrom", "ssologin" },
                { "reqtype", "PasswdCheck"},
                { "resp", "json"}
            };
            var uriString = $"{config.AuthPwdSystemHost}?{req.ToQueryString()}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString);
            var response = await httpClient.RequestAsync<AuthRes<object>>(requestMessage);
            if (response == null)
            {
                throw new AuthGetPublicKeyReturnNullErrorException("AuthCheckPassword response is null");
            }
            if (response.Code != 0)
            {
                throw new AuthPwdNotValidException(JsonSerializer.Serialize(response));

            }

            return;
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AuthGetPublicKeyReturnNullErrorException"></exception>
        /// <exception cref="AuthGetPublicKeyErrorException"></exception>
        public async Task<AuthPublicKey> AuthGetPublicKey()
        {
            var req = new Dictionary<string, string>()
            {
                { "reqtype", "do_rsa"},
                { "type", "get_pubkey"}
            };
            var uriString = $"{config.AuthVerifyHost}?{req.ToQueryString()}";
            var response = await httpClient.GetStringAsync(uriString);
            if (response == null)
            {
                throw new AuthGetPublicKeyReturnNullErrorException("AuthGetPublicKey response is null");
            }
            var authPubKey = new AuthPublicKey(response);
            if (authPubKey.ErrorCode != 0)
            {
                throw new AuthGetPublicKeyErrorException(response);
            }

            if (string.IsNullOrWhiteSpace(authPubKey.RsaVersion)
              || string.IsNullOrWhiteSpace(authPubKey.PublicKey))
            {
                throw new AuthGetPublicKeyReturnNullErrorException(response);
            }

            return authPubKey;
        }

        /// <summary>
        /// 手机号换伪码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="rsaVersion"></param>
        /// <returns></returns>
        /// <exception cref="AuthQueryTelAsReturnNullErrorException"></exception>
        /// <exception cref="AuthQueryTelAsErrorException"></exception>
        public async Task<AuthPhoneInfo> AuthGetTelAs(string phone, string rsaVersion)
        {
            var req = new Dictionary<string, string>()
            {
                { "reqtype", "query_tel_as"},
                { "tel", phone},
                { "rsa_version", rsaVersion},
                { "resp", "json"}
            };
            var uriString = $"{config.AuthMkrHost}?{req.ToQueryString()}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString);
            var response = await httpClient.RequestAsync<AuthRes<object>>(
                requestMessage);
            if (response == null)
            {
                throw new AuthQueryTelAsReturnNullErrorException("AuthGetTelAs Response is null");
            }
            if (response.Code == 0)
            {               
                var data = response.Data?.ConvertToModel<AuthPhoneInfo>();
                if (data?.TelAs == null)
                {
                    throw new AuthQueryTelAsReturnNullErrorException(JsonSerializer.Serialize(response.Msg));
                }
                return data;
            }

            if (response.Code == -2 || response.Code == -3)
            {
                // 非手机号
                return default;
            }

            throw new AuthQueryTelAsErrorException(JsonSerializer.Serialize(response.Msg));
        }

        /// <summary>
        /// 获取sessioninfo
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="AuthGetSessionInfoReturnNullException"></exception>
        /// <exception cref="AuthGetSessionInfoErrorException"></exception>
        public async Task<AuthSessionInfo> AuthGetSessionInfo(string userId)
        {
            var req = new Dictionary<string, string>()
            {
                { "reqtype", "get_sessionid_v2"},
                { "userid", userId},
                { "resp", "json"}
            };
            var uriString = $"{config.AuthVerifyHost}?{req.ToQueryString()}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString);
            var response = await httpClient.RequestAsync<AuthRes<object>>(requestMessage);
            if (response == null)
            {
                throw new AuthGetSessionInfoReturnNullException("AuthGetSessionInfo response is null");
            }
            if (response.Code != 0)
            {
                throw new AuthGetSessionInfoErrorException(JsonSerializer.Serialize(response));
            }
            var data = response.Result?.ConvertToModel<AuthSessionInfo>();
            if (string.IsNullOrWhiteSpace(data?.SessionId)
                || string.IsNullOrWhiteSpace(data?.Sign))
            {
                throw new AuthGetSessionInfoReturnNullException(JsonSerializer.Serialize(response));
            }

            return data;
        }

        public async Task<string> AuthGetPcPassport(AuthPcRequest request)
        {
            request.Reqtype = "passport_pc";
            if (!string.IsNullOrWhiteSpace(request.Securities))
            {
                request.Securities = Encoding.GetEncoding("GBK").GetString(Encoding.Default.GetBytes(request.Securities));
            }
            var reqDic = request.ToDictionary();
            var uriString = $"{config.AuthVerifyHost}?{reqDic.ToQueryString()}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString);
            var response = await httpClient.RequestAsync(requestMessage);
            if (response == null)
            {
                throw new AuthGetPcPassportReturnNullException("AuthGetPcPassport response is null");
            }

            var pcPassport = Convert.ToBase64String(response);
            return pcPassport;
        }
    }

    public class AuthRes<T>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("result")]
        public T Result { get; set; }
    }

    public class AuthSessionInfo
    {
        [JsonPropertyName("this_time")]
        public int SignTime { get; set; }

        [JsonPropertyName("expires")]
        public string Expires { get; set; }

        [JsonPropertyName("sessionid")]
        public string SessionId { get; set; }

        [JsonPropertyName("third_sign")]
        public string Sign { get; set; }
    }

    public class AuthPhoneInfo
    {
        [JsonPropertyName("akd_tel_as")]
        public int? TelAs { get; set; }
    }

    public class AuthPcRequest
    {
        [Description("userid")]
        public string Userid { get; set; }

        [Description("qsid")]
        public string QsId { get; set; }

        [Description("product")]
        public string Product { get; set; }

        [Description("version")]
        public string Version { get; set; }

        [Description("imei")]
        public string IMEI { get; set; }

        [Description("sdsn")]
        public string SDSN { get; set; }

        [Description("securities")]
        public string Securities { get; set; }

        [Description("nohqlist")]
        public string Nohqlist { get; set; }

        [Description("newwgflag")]
        public string Newwgflag { get; set; }

        [Description("reqtype")]
        public string Reqtype { get; set; }
    }

    public class AuthPublicKey
    {
        public AuthPublicKey(string xml_string)
        {
            var xe = XElement.Parse(xml_string);
            ImportConfig(xe);
        }

        public override string ToString()
        {
            return ExportConfig().ToString();
        }

        public bool IsBad { get { return ErrorCode < 0; } }
        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string RsaVersion { get; private set; }
        public string PublicKey { get; private set; }
        public string Modulus { get; private set; }
        public string PublicExponent { get; private set; }

        public XElement ExportConfig()
        {
            var xe = new XElement(nameof(AuthPublicKey));
            xe.Add(imported_xe);
            return xe;
        }

        private XElement imported_xe { get; set; }

        public void ImportConfig(XElement xe)
        {
            imported_xe = xe;
            var ret_element = xe.Element("ret");
            ErrorMessage = ret_element.Attribute("msg").Value;
            Int32 errorcode = 0;
            if (Int32.TryParse(ret_element.Attribute("code").Value, out errorcode))
            {
                ErrorCode = errorcode;
                if (ErrorCode < 0)
                    return;
            }
            var item_element = xe.Element("item");
            RsaVersion = item_element.Attribute("rsa_version")?.Value ?? "";
            PublicKey = item_element.Attribute("pubkey")?.Value ?? "";
            Modulus = item_element.Attribute("modulus")?.Value ?? "";
            PublicExponent = item_element.Attribute("publicExponent")?.Value ?? "";
        }
    }
}
