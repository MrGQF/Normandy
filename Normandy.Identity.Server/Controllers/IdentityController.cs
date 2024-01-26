using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Domain.Shared.Consts;

namespace Normandy.Identity.Server.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IConfiguration configuration;

        public IdentityController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 获取RSA公钥,base64 字符串格式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetPublicKey()
        {
            return configuration[ConfigKeys.RsaPublicKey];
        }
    }
}
