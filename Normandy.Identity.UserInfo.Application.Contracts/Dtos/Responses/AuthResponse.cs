namespace Normandy.Identity.UserInfo.Application.Contracts.Dtos.Responses
{
    public class AuthResponse 
    {
        public PassportInfo Passport { get; set; }

        public SessionInfo SessionInfo { get; set; }
    }
}
