using AutoMapper;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Requests;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Responses;

namespace Normandy.Identity.UserInfo.Application.Profiles
{
    public class AuthMapperProfile : Profile
    {
        public AuthMapperProfile()
        {
            CreateMap<UserDataRpc.SessionInfo, SessionInfo>();

            CreateMap<AuthRequest, UserDataRpc.PcPassportRequest>();

            CreateMap<AuthRequest, UserDataRpc.SessionidRequest>();
        }
    }
}
