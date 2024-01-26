using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using IdentityModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    public class TestResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {     
            var user = new TestUser
            {
                SubjectId = "1",
                Username = "user",
                Password = "123456",
                Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "gaoqifei"),
                            new Claim(JwtClaimTypes.Email, "gaoqifei@myhexin.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        }
            };

            if (context.UserName == user.Username && context.Password == user.Password)
            {
                context.Result = new GrantValidationResult(subject: user.SubjectId, authenticationMethod: OidcConstants.AuthenticationMethods.Password, user.Claims);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "密码错误");
            }

            return Task.CompletedTask;
        }
    }
}
