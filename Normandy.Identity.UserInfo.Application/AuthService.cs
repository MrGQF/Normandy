using AutoMapper;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.Domain.Shared.Exceptions;
using Normandy.Identity.UserDataRpc;
using Normandy.Identity.UserInfo.Application.Contracts;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Requests;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Responses;
using Normandy.Infrastructure.DI;

namespace Normandy.Identity.UserInfo.Application
{
    public class AuthService : IAuthService, IScopedAutoDIable
    {
        private readonly UserDataRpc.UserDataRpc.UserDataRpcClient userInfoRpcClient;
        private readonly IMapper mapper;

        public AuthService(
            UserDataRpc.UserDataRpc.UserDataRpcClient userInfoRpcClient,
            IMapper mapper)
        {
            this.userInfoRpcClient = userInfoRpcClient;
            this.mapper = mapper;
        }

        public async Task<AuthResponse> GetAuth(AuthRequest request)
        {
            var pcRequest = mapper.Map<PcPassportRequest>(request);
            var pcPassport = await GetPcPassport(pcRequest);

            var sessionRequest = mapper.Map<SessionidRequest>(request);
            var rpcSessionInfo = await GetSessionId(sessionRequest);
            var sessionInfo = mapper.Map<Contracts.Dtos.Responses.SessionInfo>(rpcSessionInfo);

            return new AuthResponse
            {
                Passport = new PassportInfo { PC = pcPassport },
                SessionInfo = sessionInfo,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> GetPcPassport(PcPassportRequest request)
        {
            var response = await userInfoRpcClient.GetPcPassportAsync(request);
            if (response.Code != (int)NormandyIdentityErrorCodes.Success)
            {
                throw new NormandyIdentityException(response.Code, response.Message);
            }

            return response.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDataRpc.SessionInfo> GetSessionId(SessionidRequest request)
        {
            var response = await userInfoRpcClient.GetSessionidAsync(request);
            if (response.Code != (int)NormandyIdentityErrorCodes.Success)
            {
                throw new NormandyIdentityException(response.Code, response.Message);
            }

            return response.Data;
        }
    }
}