using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.UserDataRpc;
using Normandy.Infrastructure.Util.Crypto;
using Normandy.Infrastructure.Util.HttpUtil;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Normandy.Identity.Sever.Application.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserDataRpc.UserDataRpc.UserDataRpcClient userInfoRpcClient;
        private readonly RSA rsa;
        private readonly IConfiguration configuration;
        private readonly IRedisDatabase redisDatabase;
        private readonly ILogger<ResourceOwnerPasswordValidator> logger;

        public ResourceOwnerPasswordValidator(
            IHttpContextAccessor httpContextAccessor,
            UserDataRpc.UserDataRpc.UserDataRpcClient userInfoRpcClient,
            RSA rsa,
            IConfiguration configuration,
            IRedisDatabase redisDatabase,
            ILogger<ResourceOwnerPasswordValidator> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userInfoRpcClient = userInfoRpcClient;
            this.rsa = rsa;
            this.configuration = configuration;
            this.redisDatabase = redisDatabase;
            this.logger = logger;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                // 密码解码
                var md5Pwd = DecryReturnMd5(context.Password);

                // 获取客户端IP
                var cip = httpContextAccessor?.HttpContext?.GetClientIPAddr();

                // 获取用户Id
                var userTuple = await GetUserIdAsync(context.UserName);
                if (userTuple.Code != (int)NormandyIdentityErrorCodes.Success)
                {
                    var message = $"{userTuple.Code}\n{userTuple.message}";
                    context.Result = new GrantValidationResult(
                        TokenRequestErrors.InvalidRequest,
                        message,
                        new Dictionary<string, object> { { nameof(Response.Code), userTuple.Code } });
                    return;
                }

                // 校验密码
                var result = await userInfoRpcClient.CheckPasswordAsync(new PasswordCheckRequest
                {
                    Userid = userTuple.Userid,
                    Md5Pwd = md5Pwd,
                    Cip = cip
                });
                if (result.Code == (int)NormandyIdentityErrorCodes.Success)
                {
                    context.Result = new GrantValidationResult(
                        subject: Convert.ToString(userTuple.Userid),
                        authenticationMethod: OidcConstants.AuthenticationMethods.Password);
                }
                else
                {
                    var message = $"{result.Code}\n{result.Message}";
                    context.Result = new GrantValidationResult(
                        TokenRequestErrors.InvalidRequest,
                        message);
                }

                context.Result.CustomResponse = new Dictionary<string, object> 
                { 
                    { nameof(Response.Code), result.Code }, 
                    { nameof(userTuple.Userid), userTuple.Userid } 
                };
            }
            catch (CryptographicException ex)
            {
                var message = $"{ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}";
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidRequest,
                    message,
                    customResponse: new Dictionary<string, object> { { nameof(Response.Code), (int)NormandyIdentityErrorCodes.PwdParseError } });
            }
            catch (Exception ex)
            {
                var message = $"{ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}";
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidRequest,
                    message,
                    customResponse: new Dictionary<string, object> { { nameof(Response.Code), (int)NormandyIdentityErrorCodes.Error } });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwd">base64 字符串格式</param>
        /// <returns></returns>
        private string DecryReturnMd5(string pwd)
        {
            var pwdByte = rsa.Decrypt(Base64Helper.Decode(pwd), RSAEncryptionPadding.Pkcs1);
            return Md5Helper.Entry(pwdByte);
        }

        public async Task<(int Code, int Userid, string message)> GetUserIdAsync(string account)
        {
            var cacheKey = $"{nameof(ResourceOwnerPasswordValidator.GetUserIdAsync)}-{account}";
            var userId = await GetCache(cacheKey);
            if (userId.HasValue)
            {
                return ((int)NormandyIdentityErrorCodes.Success, userId.Value, default);
            }

            var result = await userInfoRpcClient.GetUserInfoAsync(new UserInfoRequest
            {
                Account = account
            });
            if (result.Code != (int)NormandyIdentityErrorCodes.Success)
            {
                return (result.Code, default, result.Message);
            }
            userId = result.Data.FirstOrDefault()?.Userid;
            if (!userId.HasValue && userId.Value == 0)
            {
                return ((int)NormandyIdentityErrorCodes.UserIdNotValid, default, NormandyIdentityErrorCodes.UserIdNotValid.ToString());
            }

            await SetCache(cacheKey, userId.Value);
            return ((int)NormandyIdentityErrorCodes.Success, userId.Value, default);
        }

        private async Task<int?> GetCache(string cacheKey)
        {
            try
            {
                var userId = await redisDatabase.GetAsync<int?>(cacheKey);
                return userId;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, default, default);
            }

            return default;
        }

        private async Task SetCache(string cacheKey, int userId)
        {
            try
            {
                var expireSeconds = configuration.GetValue<int>(ConfigKeys.CacheExpireSeconds);
                var expireTimeSpan = TimeSpan.FromSeconds(expireSeconds);
                await redisDatabase.AddAsync(cacheKey, userId, expireTimeSpan);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, default, default);
            }
        }
    }
}
