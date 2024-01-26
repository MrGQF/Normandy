namespace Normandy.Identity.Domain.Shared.Exceptions
{
    /// <summary>
    /// 认证中心 获取公钥异常
    /// </summary>
    public class AuthGetPublicKeyErrorException : NormandyIdentityException
    {
        public AuthGetPublicKeyErrorException(string message) : base(message)
        {

        }
    }

    public class AuthGetPublicKeyReturnNullErrorException : NormandyIdentityException
    {
        public AuthGetPublicKeyReturnNullErrorException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// 认证中心 查询伪码异常
    /// </summary>
    public class AuthQueryTelAsReturnNullErrorException : NormandyIdentityException
    {
        public AuthQueryTelAsReturnNullErrorException(string message) : base(message)
        {

        }
    }

    public class AuthQueryTelAsErrorException : NormandyIdentityException
    {
        public AuthQueryTelAsErrorException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// 认证中心 密码校验异常
    /// </summary>
    public class AuthPwdNotValidException : NormandyIdentityException
    {
        public AuthPwdNotValidException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// 认证中心 获取SessionInfo异常
    /// </summary>
    public class AuthGetSessionInfoErrorException : NormandyIdentityException
    {
        public AuthGetSessionInfoErrorException(string message) : base(message)
        {

        }
    }

    public class AuthGetSessionInfoReturnNullException : NormandyIdentityException
    {
        public AuthGetSessionInfoReturnNullException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// 认证中心 获取通行证
    /// </summary>
    public class AuthGetPcPassportErrorException : NormandyIdentityException
    {
        public AuthGetPcPassportErrorException(string message) : base(message)
        {

        }
    }

    public class AuthGetPcPassportReturnNullException : NormandyIdentityException
    {
        public AuthGetPcPassportReturnNullException(string message) : base(message)
        {

        }
    }
}
