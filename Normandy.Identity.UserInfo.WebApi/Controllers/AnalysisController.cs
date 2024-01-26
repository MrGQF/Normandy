using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.Domain.Shared.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Normandy.Identity.UserInfo.WebApi.Controllers
{
    /// <summary>
    /// 数据分析/埋点
    /// </summary>
    public class AnalysisController : Controller
    {
        private readonly ILogger<AnalysisController> logger;
        private readonly IConfiguration configuration;

        public AnalysisController(
            ILogger<AnalysisController> logger,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// 埋点接口
        /// Template：
        /// Id:{Id} UserId:{UserId} ClientId:{ClientId} Auth:{Auth} ErrorCode:{ErrorCode} ErrorMessage:{ErrorMessage} Costs:{Costs} Type:{Type}
        /// </summary>
        /// <param name="info"></param>
        /// <param name="version"></param>
        [HttpPost]
        public void Track([FromBody][Required]IDictionary<string, object> info, [FromQuery]string version = "0")
        {
            var key = $"{ConfigKeys.TrackInfo}:{version}";
            var message = configuration.GetValue<string>(key);
            if(string.IsNullOrEmpty(message))
            {
                throw new NormandyIdentityException((int)NormandyIdentityErrorCodes.TrackInfoVersionNotExist, NormandyIdentityErrorCodes.TrackInfoVersionNotExist.ToString());
            }

            var val = info.Values.ToArray();
            logger.LogInformation(message, val);
        }
    }
}
