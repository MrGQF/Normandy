namespace Normandy.Identity.Domain.Shared.Enums
{
    public enum NormandyIdentityErrorCodes
    {       
        #region 认证中心服务 调用状态码
        AuthHttpRequestFailed = -10023000,

        AuthPwdNotValid = -10023022,

        AuthGetPublicKeyReturnNullError = -10023032,
        AuthGetPublicKeyError = -10023031,

        AuthQueryTelAsReturnNullError = -10023042,
        AuthQueryTelAsError = -10023041,

        AuthGetSessionInfoReturnNullError = -10023074,
        AuthGetSessionInfoError = -10023073,

        AuthGetPassportError = -10023011,
        AuthGetPassportReturnNull = -10023012,
        #endregion

        #region 风控
        RiskDisposed = -10021999,
        #endregion

        #region 统一登录错误
        AccountNotExist = -10041999,
        DeviceSnNotMatch,
        ProtoNotValid ,
        UserIdNotValid,
        PwdParseError,
        #endregion

        #region 基础服务错误
        ClientByIdNotFound = -10040104,
        ApiResourcesByScopeNameNotFound,
        ApiResourcesByNameNotFound,
        ApiScopesByNameNotFound,
        IdentityResourcesByScopeNameNotFound,
        AllResourcesNotFound,

        ClientFindFailed,
        ApiResourcesFindFailed,
        ApiScopesByNameFindFailed,
        IdentityResourcesByScopeNameFindFailed,
        AllResourcesFindFailed,

        UserInfoGetFailed,
        PasswordCheckFailed,
        UserInfoGetParamNotValid,
        PasswordParamNotValid,
        SessionidGetParamNotValid,
        SessionidGetFailed,
        PcPassportGetParamNotValid,
        PcPassportGetFailed,
        #endregion 基础服务错误       

        #region UserInfoWebApi
        TrackInfoVersionNotExist = -10044001,
        TrackInfoConfigNotExist = -10044000,
        #endregion
       
        Error = -1,
        Success = 0,       
    }
}
