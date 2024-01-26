namespace Normandy.Identity.Domain.Shared.Exceptions
{
    public class AccountNotExistException : NormandyIdentityException
    {
        public AccountNotExistException() : base()
        {

        }

        public AccountNotExistException(string msg) : base(msg)
        {

        }
    }
}
