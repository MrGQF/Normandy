using AutoMapper;
using Grpc.Core;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.Domain.Shared.Exceptions;
using Normandy.Identity.UserDataRpc;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Normandy.Identity.UserData.Application.Services
{
    public class UserInfoRpcService : UserDataRpc.UserDataRpc.UserDataRpcBase
    {
        private readonly UserInfoService userInfoService;
        private readonly IMapper mapper;

        public UserInfoRpcService(
            UserInfoService userInfoService,
            IMapper mapper)
        {
            this.userInfoService = userInfoService;
            this.mapper = mapper;
        }

        public override async Task<UserInfoResult> GetUserInfo(UserInfoRequest request, ServerCallContext context)
        {
            var result = new UserInfoResult
            {
                Code = (int)NormandyIdentityErrorCodes.Success
            };
            Exception err = null;
            try
            {
                if (request == null
                    || string.IsNullOrWhiteSpace(request.Account))
                {
                    result.Code = (int)NormandyIdentityErrorCodes.UserInfoGetParamNotValid;
                    return  result;
                }

                var info = await userInfoService.GetUserInfoByAccount(request.Account);
                if(info == null)
                {
                    throw new AccountNotExistException($"Account:{request.Account} not Exist");
                }

                result.Data.Add(new UserInfo
                {
                    Userid = info.Userid,
                    Name = Encoding.Default.GetString(info.Account)
                });
                return result;
            }
            catch (HttpRequestException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (TaskCanceledException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (AuthGetPublicKeyErrorException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthGetPublicKeyError;
                err = ex;
            }
            catch (AuthGetPublicKeyReturnNullErrorException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthGetPublicKeyReturnNullError;
                err = ex;
            }
            catch (AuthQueryTelAsErrorException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthQueryTelAsError;
                err = ex;
            }
            catch (AuthQueryTelAsReturnNullErrorException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthQueryTelAsReturnNullError;
                err = ex;
            }
            catch (AccountNotExistException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AccountNotExist;
                err = ex;
            }
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.UserInfoGetFailed;
                err = ex;
            }

            if (err != null)
            {
                result.Message = $"{err.GetType().Name}{err.Message}, {err.StackTrace}";
            }
            return result;
        }

        public override async Task<PasswordCheckResult> CheckPassword(PasswordCheckRequest request, ServerCallContext context)
        {
            var result = new PasswordCheckResult 
            {
                Code = (int)NormandyIdentityErrorCodes.Success
            };
            Exception err = null;
            try
            {
                if (request == null
                    || !request.Userid.HasValue
                    || request.Userid.Value == 0)
                {
                    result.Code = (int)NormandyIdentityErrorCodes.PasswordParamNotValid;
                    return result;
                }

                await userInfoService.CheckPassword(request.Md5Pwd, request.Userid.Value, request.Cip);
            }
            catch(HttpRequestException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (TaskCanceledException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (AuthPwdNotValidException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthPwdNotValid;
                err = ex;
            }
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.PasswordCheckFailed;
                err = ex;
            }

            if (err != null)
            {
                result.Message = $"{err.GetType().Name}{err.Message}, {err.StackTrace}";
            }
            return result;
        }

        public override async Task<SessionidResult> GetSessionid(SessionidRequest request, ServerCallContext context)
        {
            var result = new SessionidResult 
            {
                Code = (int)NormandyIdentityErrorCodes.Success
            };
            Exception err = null;
            try
            {
                if(request == null 
                    || string.IsNullOrWhiteSpace(request.Userid))
                {
                    result.Code = (int)NormandyIdentityErrorCodes.SessionidGetParamNotValid;
                    return result;
                }

                var authSessionInfo = await userInfoService.GetSessionInfo(request.Userid);
                result.Data = mapper.Map<SessionInfo>(authSessionInfo);
            }
            catch (HttpRequestException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (TaskCanceledException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (AuthGetSessionInfoReturnNullException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthGetSessionInfoReturnNullError;
                err = ex;
            }
            catch(AuthGetSessionInfoErrorException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthGetSessionInfoError;
                err = ex;
            }
            catch(Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.SessionidGetFailed;
                err = ex;
            }

            if (err != null)
            {
                result.Message = $"{err.GetType().Name}{err.Message}, {err.StackTrace}";
            }
            return result;
        }

        public override async Task<PcPassportResult> GetPcPassport(PcPassportRequest request, ServerCallContext context)
        {
            var result = new PcPassportResult 
            {
                Code = (int)NormandyIdentityErrorCodes.Success
            };
            Exception err = null;
            try
            {
                if(request == null
                    || string.IsNullOrWhiteSpace(request.Userid))
                {
                    result.Code = (int)NormandyIdentityErrorCodes.PcPassportGetParamNotValid;
                    return result;
                }

                var authReq = mapper.Map<AuthPcRequest>(request);
                var pcPassport = await userInfoService.GetPcPassport(authReq);
                result.Data = pcPassport;
            }
            catch (HttpRequestException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch(TaskCanceledException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthHttpRequestFailed;
                err = ex;
            }
            catch (AuthGetPcPassportReturnNullException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthGetPassportReturnNull;
                err = ex;
            }
            catch (AuthGetPcPassportErrorException ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.AuthGetPassportError;
                err = ex;
            }
            catch (Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.PcPassportGetFailed;
                err = ex;
            }

            if (err != null)
            {
                result.Message = $"{err.GetType().Name}{err.Message}, {err.StackTrace}";
            }
            return result;
        }
    }
}
