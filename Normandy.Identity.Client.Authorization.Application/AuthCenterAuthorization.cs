using Normandy.Identity.Client.Authorization.Application.Contracts;
using Normandy.Identity.Client.Authorization.Application.Contracts.Requests;
using Normandy.Identity.Client.Authorization.Application.Contracts.Responses;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Authorization.Application
{
    /// <summary>
    /// 认证中心 授权业务
    /// </summary>
    public class AuthCenterAuthorization : IAuthorization
    {
        public Task<CloudInfo> DownloadSelfCodeDataAsync(string bizName, string path, long version, string clientType)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<CookieInfo>> GetCookie(CookieRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<string>> GetPassport(PassportRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<object>> LogOut()
        {
            throw new System.NotImplementedException();
        }

        public Task<CloudInfo> PollCloundData(string biz, string path, long version)
        {
            throw new System.NotImplementedException();
        }

        public Task<CloudInfo> PushCloundData(string biz, string path, long version, string Filename, byte[] file)
        {
            throw new System.NotImplementedException();
        }

        public Task<CloudInfo> PushSelfCodeToCloudAsync(string bizName, string path, long version, byte[] file, string clientType)
        {
            throw new System.NotImplementedException();
        }
    }
}
