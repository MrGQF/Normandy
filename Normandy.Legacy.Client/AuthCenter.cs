using Normandy.Legacy.Client.Interfaces;
using Normandy.Legacy.Client.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Normandy.Legacy.Client
{
    public delegate Task<UserToken> AuthAsyncDelegate(String account, String password);

    public static class AuthCenter
    {
        public static AuthAsyncDelegate AuthAsync { get; set; } = AuthB2CAsync;

        public static async Task<UserToken> AuthB2CAsync(String account, String password)
        {
            try
            {
                // Logger.Info("Get the Public Key");
                var public_key = await GetAuthPublicKeyAsync();
                if (public_key == null)
                {
                    // Logger.Error("Get the Public Key: Null");
                    return null;
                }
                var info = await GetAuthInfoAsync(account, password, public_key);
                if (info == null)
                {
                    ErrorMessage.Add("GetAuthInfoAsync return Error");
                    // Logger.Error("GetAuthInfoAsync return Error");
                    return null;
                }

                var passport = await GetAuthPassportAsync(info);
                if (passport == null)
                {
                    ErrorMessage.Add("GetAuthPassportAsync return Error");
                    // Logger.Error("GetAuthPassportAsync return Error");
                    return null;
                }
                var user_token = new UserToken()
                {
                    PublicKey = public_key,
                    Info = info,
                    Passport = passport
                };

                return user_token;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<String> ErrorMessage = new List<String>();
        public static String AuthCenterUrl { get; set; } = "http://auth.10jqka.com.cn/verify2";
        public static String AuthCenterPublicKeyUrl { get; } = $"{AuthCenterUrl}?reqtype=do_rsa&type=get_pubkey";

        public static async Task<AuthPublicKey> GetAuthPublicKeyAsync()
        {
            try
            {
                String response_string = await http_client.GetStringAsync(AuthCenterPublicKeyUrl);
                ErrorMessage.Add("public_key=response_string");
                var auth_public_key = new AuthPublicKey(response_string);
                return auth_public_key;
            }
            catch (Exception ex)
            {
                // Logger.Error($"GetAuthPublicKeyAsync：{ex.Message} {ex.StackTrace}");
                return null;
            }
        }

        public static async Task<AuthInfo> GetAuthInfoAsync(String account, String password, AuthPublicKey public_key)
        {
            try
            {
                var rsa_provider = new RSACryptoServiceProvider();
                var dp = rsa_provider.ExportParameters(false);

                var gbk_account = HevoHelper.GbkEncoding.GetBytes(account);
                var gbk_password = HevoHelper.GbkEncoding.GetBytes(password);
                ErrorMessage.Add($"gbk_password");
                var encrypted_gbk_account = HevoHelper.Encrypt(gbk_account, public_key.ToRsaParameters(), false);
                var encrypted_gbk_password = HevoHelper.Encrypt(gbk_password, public_key.ToRsaParameters(), false);

                var base64_encrypted_gbk_account = HevoHelper.HexinBase64Encode(encrypted_gbk_account);
                var base64_encrypted_gbk_password = HevoHelper.HexinBase64Encode(encrypted_gbk_password);

                var encrypt_base64_encrypted_gbk_account = HevoHelper.EncodeUrl(base64_encrypted_gbk_account);
                var encrypt_base64_encrypted_gbk_password = HevoHelper.EncodeUrl(base64_encrypted_gbk_password);

                var kvs = new SortedDictionary<String, String>()
                {
                    {"reqtype", "unified_login" },
                    {"account", $"{encrypt_base64_encrypted_gbk_account}" },
                    {"passwd", $"{encrypt_base64_encrypted_gbk_password}" },
                    {"rsa_version", $"{public_key.RsaVersion}" },
                };
                ErrorMessage.Add($" make kvs sucess");
                var query_string = HevoHelper.MakeQueryString(kvs);
                ErrorMessage.Add($"GetAuthInfoAsync query_string={query_string}");
                var gbk_query_string = HevoHelper.GbkEncoding.GetBytes(query_string);
                var response_body = await Info_client.PostAsyncBytes(AuthCenterUrl, gbk_query_string);
                var content = await response_body.Content.ReadAsByteArrayAsync();
                var rsp_string = HevoHelper.GbkEncoding.GetString(content);
                ErrorMessage.Add(rsp_string);
                // Logger.Info($"GetAuthInfoAsync Reply：{rsp_string}");
                var auth_login_info = new AuthInfo(rsp_string);
                auth_login_info.InputPassword = password;
                auth_login_info.InputUserName = account;
                return auth_login_info;
            }
            catch (Exception ex)
            {
                // Logger.Error($"GetAuthInfoAsync：{ex.Message} {ex.StackTrace}");
                return null;
            }
        }
        public static NetworkInterface[] CurrentNetworkInterfaces { get; set; } = null;
        public static String GetMacAddress()
        {
            if (CurrentNetworkInterfaces == null)
            {
                CurrentNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            }
            var nics = CurrentNetworkInterfaces;
            var mac_address = String.Empty;

            if (nics.Length > 0)
            {
                var nic = nics[0];
                var ip_propertise = nic.GetIPProperties();
                mac_address = nic.GetPhysicalAddress().ToString();
            }

            // 增加横线
            var tokens = new List<string>();
            for (int i = 0; i < mac_address.Length; i += 2)
            {
                tokens.Add($"{mac_address[i]}{mac_address[i + 1]}");
            }

            var fixed_mac_address = String.Join("-", tokens);
            return fixed_mac_address;
        }

        public static String GetImeiForAuth()
        {
            var mac_addr = GetMacAddress();
            try
            {
                var source = $"{mac_addr}000000000000000000000000000000";
                var md5 = new MD5CryptoServiceProvider();
                byte[] result = Encoding.ASCII.GetBytes(source);
                byte[] output = md5.ComputeHash(result);
                var md5_string_with_gang = BitConverter.ToString(output);
                var md5_string = String.Join("", md5_string_with_gang.Split('-'));
                return md5_string;
            }
            catch (System.Exception ex)
            {
                var md5_string = String.Join("", mac_addr.Split('-'));
                // 补全md5值
                return $"{md5_string}AABBCCDDEEFFGGHHIIGG";
            }
        }
        public static String GetExeVersion()
        {
            var exe_path = Assembly.GetEntryAssembly().Location;
            var version = FileVersionInfo.GetVersionInfo(exe_path).ProductVersion;
            return version;
        }
        public static async Task<HttpResponseMessage> PostAsyncBytes(this HttpClient httpclient, string requestUri, byte[] gbk_query_string)
        {
            try
            {
                var post_content = new ByteArrayContent(gbk_query_string);
                post_content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                HttpResponseMessage response_body = null;

                // 直接访问
                response_body = await httpclient.PostAsync(requestUri, post_content);

                return response_body;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task<String> GetAuthPassportAsync(AuthInfo auth_login_info)
        {
            String rsp_string = String.Empty;
            try
            {
                if (auth_login_info.IsBad)
                {
                    ErrorMessage.Add("GetAuthPassportAsync auth_login_info");
                    return String.Empty;
                }

                var imei = GetImeiForAuth();
                var ver = GetExeVersion();
                var qsid = "800";
                var kvs = new SortedDictionary<String, String>()
                {
                    { "reqtype", "verify" },
                    { "product", "S01" },
                    { "imei", imei },
                    { "userid", auth_login_info.UserID },
                    { "sessionid", auth_login_info.SessionID },
                    { "qsid",qsid},
                    { "version", ver },
                    { "sdsn", "" },
                    { "securities", "同花顺远航版" },
                    { "nohqlist", "0" },
                };

                var query_string = HevoHelper.MakeQueryString(kvs);
                ErrorMessage.Add($"GetAuthPassportAsync query_string={query_string}");
                var gbk_query_string = HevoHelper.GbkEncoding.GetBytes(query_string);
                var rsp = await passport_client.PostAsyncBytes(AuthCenterUrl, gbk_query_string);
                var content = await rsp.Content.ReadAsByteArrayAsync();
                rsp_string = HevoHelper.GbkEncoding.GetString(content);

                return rsp_string;
            }
            catch (Exception e)
            {
                ErrorMessage.Add($"GetAuthPassportAsync:{e.Message} Stack:{e.StackTrace} ---rsp_string:{rsp_string}");
                return null;
            }
        }

        private static HttpClient Info_client { get; } = new HttpClient();
        private static HttpClient http_client { get; } = new HttpClient();
        private static HttpClient passport_client { get; } = new HttpClient();

    }

    public class AuthPublicKey : IConfigable
    {
        public AuthPublicKey(String xml_string)
        {
            var xe = XElement.Parse(xml_string);
            ImportConfig(xe);
        }

        public RSAParameters ToRsaParameters()
        {
            var rsa_parameters = new RSAParameters()
            {
                Modulus = HevoHelper.HexStringToBytes(Modulus),
                Exponent = HevoHelper.HexStringToBytes(PublicExponent),
            };
            return rsa_parameters;
        }

        public override string ToString()
        {
            return ExportConfig().ToString();
        }

        public bool IsBad { get { return ErrorCode < 0; } }
        public Int32 ErrorCode { get; private set; }
        public String ErrorMessage { get; private set; }
        public String RsaVersion { get; private set; }
        public String PublicKey { get; private set; }
        public String Modulus { get; private set; }
        public String PublicExponent { get; private set; }

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

    public class AuthInfo : IConfigable
    {
        public AuthInfo(String xml_string)
        {
            var xe = XElement.Parse(xml_string);
            imported_xe = xe;
            ImportConfig(xe);
        }

        public AuthInfo(
            string account,
            string userId,
            string sessionId,
            string Expires,
            string ThirdSign)
        {
            UserID = userId;
            Account = account;
            SessionID = sessionId;
            this.Expires = Expires;
            this.ThirdSign = ThirdSign;
        }

        public bool IsBad { get { return ErrorCode < 0; } }
        public String InputUserName { get; set; }
        public String InputPassword { get; set; }

        public override string ToString()
        {
            return ExportConfig().ToString();
        }

        public Int32 ErrorCode { get; protected set; }
        public String ErrorMessage { get; protected set; }
        public String UserID { get; protected set; }
        public String Account { get; protected set; }
        public String SessionID { get; protected set; }
        public String RsaVersion { get; protected set; }
        public String Expires { get; protected set; }
        public String ThirdSign { get; protected set; }
        public String Password { get; protected set; }

        private String _UserAvatarUrl = String.Empty;

        public String UserAvatarUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_UserAvatarUrl))
                {
                    var id = Convert.ToInt32(UserID) % 10000;
                    var id_str = String.Format("{0:0000}", id);
                    var url = $"http://u.thsi.cn/avatar/{id_str}/{UserID}.gif";
                    return url;
                }
                else
                {
                    return _UserAvatarUrl;
                }
            }
            set
            {
                _UserAvatarUrl = value;
            }
        }

        protected XElement imported_xe { get; set; }

        public XElement ExportConfig()
        {
            var xe = new XElement(nameof(AuthInfo));
            xe.Add(imported_xe);
            return xe;
        }

        public void ImportConfig(XElement xe)
        {
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
            UserID = item_element.Attribute("userid")?.Value ?? "";
            Account = item_element.Attribute("account")?.Value ?? "";
            SessionID = item_element.Attribute("sessionid")?.Value ?? "";
            RsaVersion = item_element.Attribute("rsa_version")?.Value ?? "";
            Expires = item_element.Attribute("expires")?.Value ?? "";
            ThirdSign = item_element.Attribute("third_sign")?.Value ?? "";
        }
    }

    internal static class VerifyType
    {
        internal const String Quote = "1"; //行情
        internal const String Download = "2"; //下载
        internal const String Info = "3"; //资讯
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AuthPackHead
    {
        public short TotalSize;         //整个通行证的长度
        public byte AuthType;           //认证分类
        public short SignatureLength;   //数字签名的长度
    }

    public class UserToken
    {
        public String BranchID { get; set; }
        public AuthPublicKey PublicKey { get; set; }
        public String Passport { get; set; }
        public AuthInfo Info { get; set; }


    }
}