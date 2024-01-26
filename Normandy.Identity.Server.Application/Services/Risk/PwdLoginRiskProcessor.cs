using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.Server.Application.Contracts.Dtos;
using Normandy.Identity.Server.Application.Contracts.Requests;
using Normandy.Identity.Sever.Application.Services.Risk;
using Normandy.Infrastructure.DI;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.HttpUtil;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace Normandy.Identity.Server.Application.Services.Risk
{
    public class PwdLoginRiskProcessor : RiskProcessorBase
    {
        public PwdLoginRiskProcessor(
            IHttpClientFactory clientFactory, 
            IConfiguration configuration, 
            ILogger<PwdLoginRiskProcessor> logger) 
            : base(clientFactory, configuration, logger)
        {
        }

        public override int BeforeEventModelId => 1000;

        public override int InnerEventModelId => 1001;

        public override int AfterEventModelId => 1002;

        public override RiskEventInfo ParseRequest(HttpContext context, IdentityHeader identityHeader, string reqBody)
        {
            var reqDic = reqBody.ParseQueryString();
            if (reqDic == null
                || !reqDic.Any()
                || !reqDic.TryGetValue(OidcConstants.TokenRequest.GrantType, out var grantType)
                || !reqDic.TryGetValue(OidcConstants.TokenRequest.UserName, out var userName)
                || !reqDic.TryGetValue(OidcConstants.TokenRequest.Password, out var pwd))
            {
                throw new ArgumentNullException(nameof(reqBody));
            }

            int loginType;
            if (grantType.ToString() == GrantType.ResourceOwnerPassword)
            {
                loginType = (int)LoginType.Pwd;
            }
            else
            {
                return default;
            }

            return new RiskEventInfo
            {
                TraceId = identityHeader?.TraceId,
                Account = userName?.ToString(),
                Password = pwd?.ToString(),
                Did = identityHeader?.DeviceSn,
                ClientIp = context.GetClientIPAddr(),
                AppId = identityHeader?.AppId,
                AppVersion = identityHeader?.AppVersion,
                AppType = identityHeader?.AppId,
                Sdtis = string.Empty,
                LoginType = loginType,
                SdkVersion = identityHeader?.SDKVersion,
                AppSign = identityHeader?.AppSign
            };   
        }

        public override void ParseResponse(RiskEventInfo data, string resBody)
        {
            if(string.IsNullOrWhiteSpace(resBody))
            {
                return;
            }

            var res = JsonSerializer.Deserialize<IdentityTokenResponse>(resBody);
            data.Code = Convert.ToString(res.Code);
            data.UserId = Convert.ToString(res.Userid);
        }
    }
}
