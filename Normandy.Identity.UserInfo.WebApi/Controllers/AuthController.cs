using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.UserInfo.Application.Contracts;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Requests;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Responses;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Identity.UserInfo.WebApi.Controllers
{
    /// <summary>
    /// 认证相关
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IAuthService authService;

        public AuthController(
            IAuthService authService)
        {
            this.authService = authService;
        }

        [Authorize]
        [HttpPost]
        public Task<AuthResponse> GetAuth([FromBody][Required]AuthRequest request)
        {
            var userId = HttpContext.User.Claims.Where(t => t.Type == JwtClaimTypes.Subject).Select(t => t.Value).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(NormandyIdentityErrorCodes.UserIdNotValid.ToString());
            }
            request.Userid = userId;
            

            return authService.GetAuth(request);
        }        
    }
}
