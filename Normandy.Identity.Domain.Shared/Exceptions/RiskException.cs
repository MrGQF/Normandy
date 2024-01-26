namespace Normandy.Identity.Domain.Shared.Exceptions
{
    public class RiskCheckUrlEmptyException : NormandyIdentityException
    {
        public RiskCheckUrlEmptyException() : base()
        {

        }

        public RiskCheckUrlEmptyException(string msg) : base(msg)
        {

        }
    }
}
