using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Infrastructure.Util.HttpUtil;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    public class CustomTokenValidator : ICustomTokenValidator
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CustomTokenValidator(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }


        public Task<TokenValidationResult> ValidateAccessTokenAsync(TokenValidationResult result)
        {
            result = Validate(result);
            return Task.FromResult(result);
        }

        public Task<TokenValidationResult> ValidateIdentityTokenAsync(TokenValidationResult result)
        {
            result = Validate(result);
            return Task.FromResult(result);
        }

        private TokenValidationResult Validate(TokenValidationResult result)
        {


            // 判断设备指纹
            var deviceSN = httpContextAccessor?.HttpContext?.Request.GetHeaderValueRaw(Consts.IdentityHeaderKey_DeviceSn);
            var deviceCliam = result.Claims.FirstOrDefault(t => t.Type == Consts.IdentityHeaderKey_DeviceSn);
            if (string.IsNullOrWhiteSpace(deviceSN)
                || string.IsNullOrWhiteSpace(deviceCliam?.Value))
            {
                return result;
            }
            if (deviceSN != deviceCliam.Value)
            {
                result.IsError = true;
                result.Error = NormandyIdentityErrorCodes.DeviceSnNotMatch.ToString();
                return result;
            }

            return result;
        }
    }
}
