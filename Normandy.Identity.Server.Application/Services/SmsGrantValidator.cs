using IdentityServer4.Validation;
using System;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    public class SmsGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "sms";

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
