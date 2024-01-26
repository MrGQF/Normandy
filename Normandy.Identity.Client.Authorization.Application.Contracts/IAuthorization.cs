using Normandy.Identity.Client.Authorization.Application.Contracts.Requests;
using Normandy.Identity.Client.Authorization.Application.Contracts.Responses;
using System;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Authorization.Application.Contracts
{
    /// <summary>
    /// 授权
    /// </summary>
    public interface IAuthorization
    {
        /// <summary>
        /// 获取通行证
        /// </summary>
        /// <param name="request"></param>
        Task<Result<string>> GetPassport(PassportRequest request);

        /// <summary>
        /// 获取Cookie
        /// </summary>
        Task<Result<CookieInfo>> GetCookie(CookieRequest request);

        /// <summary>
        /// 注销登录
        /// </summary>
        Task<Result<object>> LogOut();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="biz"></param>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        Task<CloudInfo> PollCloundData(String biz, String path, Int64 version);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="biz"></param>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <param name="Filename"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<CloudInfo> PushCloundData(String biz, String path, Int64 version, String Filename, Byte[] file);

        /// <summary>
        /// 获取云端App端自选股数据
        /// </summary>
        /// <param name="bizName"></param>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        Task<CloudInfo> DownloadSelfCodeDataAsync(string bizName, string path, long version, string clientType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizName"></param>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <param name="file"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        Task<CloudInfo> PushSelfCodeToCloudAsync(string bizName, string path, long version, byte[] file, string clientType);
    }
}
