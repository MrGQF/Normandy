using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Normandy.Identity.Domain.Shared.Consts;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    /// <summary>
    /// 自定义申明
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProfileService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var deviceSN = httpContextAccessor?.HttpContext?.Request.Headers[Consts.IdentityHeaderKey_DeviceSn];
            if (string.IsNullOrWhiteSpace(deviceSN))
            {
                deviceSN = Consts.IdentityHeaderKey_DeviceSn;
            }

            var claims = new List<Claim> {
                new Claim(Consts.IdentityHeaderKey_DeviceSn, deviceSN),
            };

            context.IssuedClaims.AddRange(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
