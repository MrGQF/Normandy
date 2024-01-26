using Normandy.Identity.UserData.Domain.Models;
using Normandy.Infrastructure.DI;
using Normandy.Infrastructure.Repository;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.Crypto;
using RSAExtensions;
using System.Text;
using System.Threading.Tasks;

namespace Normandy.Identity.UserData.Application.Services
{
    public class UserInfoService : IScopedAutoDIable
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AuthCenterService authCenterService;

        public UserInfoService(
            IUnitOfWork unitOfWork,
            AuthCenterService authCenterService)
        {
            this.unitOfWork = unitOfWork;
            this.authCenterService = authCenterService;
        }

        /// <summary>
        /// 获取Pc 通行证
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<string> GetPcPassport(AuthPcRequest request)
        {
            return authCenterService.AuthGetPcPassport(request);
        }

        /// <summary>
        /// 获取sessioninfo
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<AuthSessionInfo> GetSessionInfo(string userId)
        {
            return authCenterService.AuthGetSessionInfo(userId);
        }

        /// <summary>
        /// 校验密码
        /// </summary>
        /// <param name="md5Pwd"></param>
        /// <param name="userId"></param>
        /// <param name="cIp"></param>
        /// <returns></returns>
        public Task CheckPassword(
            string md5Pwd,
            int userId,
            string cIp)
        {
            return authCenterService.AuthCheckPassword(md5Pwd, userId, cIp);
        }

        /// <summary>
        /// 根据账号查询用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<VerifyUserinfo> GetUserInfoByAccount(string account)
        {
            VerifyUserinfo info = default;
            if (RegexMatch.IsEmail(account))
            {
                // 邮箱 查询用户信息
                info = await GetVerifyUserinfoByEmail(account);
            }

            if (RegexMatch.IsNumber(account))
            {
                var telAsRes = await GetTelAsFromAuth(account);
                if (telAsRes != null)
                {
                    // 手机号 查询用户信息
                    info = await GetVerifyUserinfoByTelAs(telAsRes.Value);
                }
            }

            if (info == null)
            {
                // 账号 查询用户信息
                info = await GetVerifyUserinfoByAccount(account);
            }

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public async Task<VerifyUserinfo> GetVerifyUserinfoByAccount(string account)
        {
            return await unitOfWork.GetRepository<VerifyUserinfo>().FindOrDefaultAsync(t => t.Nocase == account);
        }

        /// <summary>
        /// 根据用户Id 查询用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<VerifyUserinfo> GetVerifyUserinfoByUserId(int userId)
        {
            return await unitOfWork.GetRepository<VerifyUserinfo>().FindOrDefaultAsync(t => t.Userid == userId);
        }

        /// <summary>
        /// 邮箱 查询用户信息
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<VerifyUserinfo> GetVerifyUserinfoByEmail(string email)
        {
            var appPaEmail = await unitOfWork.GetRepository<AppPaEmail>().FindOrDefaultAsync(t => t.Email == email);
            if (appPaEmail == null)
            {
                return default;
            }

            return await GetVerifyUserinfoByUserId(appPaEmail.Userid);
        }

        /// <summary>
        /// 伪码查询用户信息
        /// </summary>
        /// <param name="telAs"></param>
        /// <returns></returns>
        public async Task<VerifyUserinfo> GetVerifyUserinfoByTelAs(int telAsInt)
        {
            var telAs = telAsInt.ToString();

            // 认证中心 app_pa_mobile 和 app_pa_mobile2 中 伪码 以“#”前后缀
            telAs = "#" + telAs + "#";

            // 根据国内手机号 查询用户信息
            var appPaMobile = await unitOfWork.GetRepository<AppPaMobile>().FindOrDefaultAsync(t => t.Mobile == telAs);
            if (appPaMobile != null)
            {
                return await GetVerifyUserinfoByUserId(appPaMobile.Userid);
            }

            // 根据国外手机号 查询用户信息
            var appPaMobile2 = await unitOfWork.GetRepository<AppPaMobile2>().FindOrDefaultAsync(t => t.Mobile == telAs);
            if (appPaMobile2 != null)
            {
                return await GetVerifyUserinfoByUserId(appPaMobile2.Userid);
            }

            return default;
        }

        /// <summary>
        /// 认证中心 手机号查询伪码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<int?> GetTelAsFromAuth(string phone)
        {
            // 获取公钥
            var pubKeyInfo = await authCenterService.AuthGetPublicKey();

            // 手机号加密
            var encryByteArray = RSAHelper.Encrypt(phone, pubKeyInfo.PublicKey, type: RSAKeyType.Pkcs8);
            var encryEncode = Base64Helper.Encode(encryByteArray);

            // 查询伪码
            var telAsRes = await authCenterService.AuthGetTelAs(encryEncode, pubKeyInfo.RsaVersion);
            return telAsRes?.TelAs;
        }
    }
}
