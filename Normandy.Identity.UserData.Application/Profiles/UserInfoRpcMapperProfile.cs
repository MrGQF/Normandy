using AutoMapper;
using Normandy.Identity.UserData.Application.Services;
using Normandy.Identity.UserDataRpc;
using System;

namespace Normandy.Identity.UserData.Application.Profiles
{
    public class UserInfoRpcMapperProfile : Profile
    {
        public UserInfoRpcMapperProfile()
        {
            CreateMap<PcPassportRequest, AuthPcRequest>();

            CreateMap<AuthSessionInfo, SessionInfo>()
                .ForMember(Des => Des.SignTime, opt => opt.MapFrom(src => Convert.ToString(src.SignTime)));
        }
    }
}
