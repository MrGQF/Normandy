using Normandy.Identity.UserInfo.Application.Contracts.Dtos.Requests;
using System.Threading.Tasks;

namespace Normandy.Identity.UserInfo.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnalysisService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackInfo"></param>
        /// <returns></returns>
        Task Track(TrackInfo trackInfo);
    }
}
