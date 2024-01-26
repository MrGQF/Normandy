using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Normandy.Identity.Client.Authentication.Application.Contracts;
using Normandy.Identity.Client.Authentication.Application.Contracts.Requests;
using Normandy.Identity.Client.Authentication.Application.Contracts.Responses;
using Normandy.Identity.Client.Domain.DomainServices;
using Normandy.Identity.Client.Domain.Extensions;
using Normandy.Identity.Client.Domain.Requests;
using Normandy.Identity.Client.Domain.Responses;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Identity.Client.Domain.Shared.Consts;
using Normandy.Infrastructure.Config;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Authentication.Application
{
    /// <summary>
    /// 安全V2 认证业务
    /// </summary>
    public class SecurityAuthentication : IAuthentication
    {
        private readonly AuthenticationDomainService domainService;
        private readonly RiskDomainService riskDomainService;
        private readonly IMemoryCache memoryCache;
        private readonly IDataProtector protector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainService"></param>
        /// <param name="riskDomainService"></param>
        /// <param name="memoryCache"></param>
        /// <param name="provider"></param>
        public SecurityAuthentication(
            AuthenticationDomainService domainService,
            RiskDomainService riskDomainService,
            IMemoryCache memoryCache,
            IDataProtectionProvider provider)
        {
            this.domainService = domainService;
            this.riskDomainService = riskDomainService;
            this.memoryCache = memoryCache;

            protector = provider.CreateProtector(memoryCache.Get<string>(CacheKeys.AppsecretKey));
        }

        /// <summary>
        /// 使用公钥对用户输入的密码加密,返回base64字符串
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private string CryptoPwd(string publicKey, string pwd)
        {

            var result = RSAHelper.Encrypt(pwd, publicKey);
            return Base64Helper.Encode(result);
        }

        /// <summary>
        /// 获取公钥
        /// 返回base64 编码的字符串
        /// </summary>
        /// <returns></returns>
        private Task<Response<string>> GetPublicKey()
        {
            return domainService.GetPublicKey();
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private string Sign(string account, string pwd)
        {
            var signDic = memoryCache.Get<Dictionary<string, string>>(CacheKeys.HttpCommonOptions);

            var request = new SSOLoginRequest
            {
                Account = account,
                Password = pwd
            };
            var reqDic = ClassExtensions.ToDictionary(request);

            signDic = signDic.Concat(reqDic).ToDictionary(k => k.Key, v => v.Value);

            return SignHelper.Sign(signDic);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cryptoPwd"></param>
        /// <param name="sign"></param>
        /// <param name="successHandler"></param>
        /// <returns></returns>
        private async Task<Response<SecuritySSOLoginResponse>> Login(
            string cryptoPwd,
            string sign,
            SSOLoginRequest request,
            Action<SSOLoginRequest, SecuritySSOLoginResponse> successHandler)
        {
            var response = await domainService.SSOLogin(new SecuritySSOLoginRequest
            {
                Account = request.Account,
                Password = cryptoPwd,
                Sign = sign
            });

            if (response.Code == HttpCode.Ok)
            {
                successHandler?.Invoke(request, response.Data);
            }

            return response;
        }

        /// <summary>
        /// 登录成功操作
        /// </summary>
        /// <param name="response"></param>
        /// <param name="userInfo"></param>
        private void LoginSuccessHandler(SSOLoginRequest userInfo, SecuritySSOLoginResponse response)
        {
            // 保存令牌缓存
            CommonExtensions.RefreshTokenCache(memoryCache, response?.TokenInfo);

            // 保存session
            CommonExtensions.RefreshSessionCache(memoryCache, response?.SessionInfo);

            // 保存用户信息
            CommonExtensions.RefreshUserInfoCache(memoryCache, response?.UserInfo);
            SaveUserInfo(userInfo);
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        private void SaveUserInfo(SSOLoginRequest userInfo)
        {
            var cryptoPwd = protector.Protect(userInfo.Password);
            var filePath = memoryCache.Get<string>(CacheKeys.ConfigFilePathKey);
            var needUpdatedPairs = new Dictionary<string, object>
            {
                { ConstKeys.ConfigUserAccountKey, userInfo.Account},
                { ConstKeys.ConfigUserPwdKey, cryptoPwd}
            };

            JsonFileHelper.Update(filePath, needUpdatedPairs);
        }

        /// <summary>
        /// 风控结果上报
        /// </summary>
        /// <param name="result"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task RiskEventCheck(Result<SSOLoginResponse> result, string account)
        {
            try
            {
                var commons = memoryCache.Get<Dictionary<string, string>>(CacheKeys.HttpCommonOptions);
                await riskDomainService.RiskEventCheck(new RiskEventCheckRequest
                {
                    EventModelId = "2",
                    Sync = false,
                    EventData = new EventData
                    {
                        UserId = result?.Data?.UserId == null ? string.Empty : Convert.ToString(result?.Data?.UserId.Value),
                        Account = account,
                        Cip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString(),
                        Did = commons.GetValueOrDefault("h-devicesn"),
                        LoginTime = DateTime.Now,
                        Scene = "login",
                        Status = "200",
                        ErrorType = Convert.ToString(result.Code),
                        AppId = commons.GetValueOrDefault("h-appid"),
                        Version = commons.GetValueOrDefault("h-version"),
                        TraceId = Guid.NewGuid().ToString(),
                        LoginType = "3",
                        CType = "PC"
                    }

                });
            }
            catch(Exception ex)
            {

            }     
        }

        /// 账密登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<SSOLoginResponse>> SSOLogin(SSOLoginRequest request)
        {
            var result = new Result<SSOLoginResponse> 
            {
                Flag = -1
            };
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Account) || string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new ArgumentException(nameof(SSOLoginRequest));
                }

                // 获取公钥
                var pubKeyRes = await GetPublicKey().ConfigureAwait(false);
                if (pubKeyRes.Code != HttpCode.Ok)
                {
                    result.Code = pubKeyRes.Code;
                    result.StackTrace = pubKeyRes.Message;
                    result.Message = pubKeyRes.Message;
                    return result;
                }

                // 公钥加密
                var cryptoPwd = CryptoPwd(pubKeyRes.Data, request.Password);

                // 签名
                var sign = Sign(request.Account, cryptoPwd);

                // 登录
                var loginRes = await Login(cryptoPwd, sign, request, (a, b) => LoginSuccessHandler(a, b));
                result.Code = loginRes.Code;
                result.StackTrace = loginRes.Message;
                result.Message = loginRes.Message;

                if (loginRes.Code != HttpCode.Ok)
                {
                    return result;
                }

                // 获取session
                result.Flag = 0;
                result.Data = new SSOLoginResponse
                {
                    NickName = loginRes?.Data?.UserInfo.NickName,
                    UserId = loginRes?.Data?.UserInfo?.UserId,
                    SessionId = loginRes?.Data?.SessionInfo?.SessionId,
                    SignTime = loginRes?.Data?.SessionInfo?.SignTime,
                    Expires = loginRes?.Data?.SessionInfo?.Expires,
                    Sign = loginRes?.Data?.SessionInfo?.Sign
                };
                return result;
            }
            catch (Exception ex)
            {
                result.Code = HttpCode.InnerError;
                result.Message = ex.Message;
                result.StackTrace = ex.StackTrace;
            }
            finally
            {
                //await RiskEventCheck(result, request.Account);
            }

            return result;
        }
    }
}
