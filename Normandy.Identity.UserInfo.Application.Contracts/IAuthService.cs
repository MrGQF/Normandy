using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Requests;
using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Responses;
using System.Threading.Tasks;

namespace Normandy.Identity.UserInfo.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AuthResponse> GetAuth(AuthRequest request);
    }
}
