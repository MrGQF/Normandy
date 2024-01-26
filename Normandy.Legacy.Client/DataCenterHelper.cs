using Google.Protobuf;
using Normandy.Legacy.Client;
using Normandy.Legacy.Client.Protocols;
using Normandy.Legacy.Client.Protos;
using Normandy.Identity.Client.Authorization.Application.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Normandy.Legacy.Client
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataCenterHelper
    {
        private static readonly String BASE_URL = "http://cs.10jqka.com.cn/multiStorage";
        private static readonly String SHARE_URL = "http://cs.10jqka.com.cn/fileStorage/upload";

        public static async Task<CloudInfo> PollCloundData(HttpClient client, AuthInfo info, String biz, String path, Int64 version)
        {
            var kvs = new Dictionary<String, String>()
            {
               { "reqtype", "download" },
               { "userid", info?.UserID },
                { "storepath", path },
                { "sessionid", info.SessionID },
                { "expires", info ?.Expires},
                { "token", info ?.ThirdSign },
                { "appname", biz },
                { "storetype", "2" },
                { "clienttype", "hevo_pc"},
                { "version", version.ToString()}
            };
            var header = await PostRequestAsync(client, BASE_URL, kvs);
            if (header == null)
            {
                return new CloudInfo() { Code = HttpStatusCode.NotFound };
            }
            var rsp_string = await header.Content.ReadAsByteArrayAsync();
            var data = new CloudInfo();
            data.Code = header.StatusCode;
            if (data.Code != HttpStatusCode.OK)
            {
                return data;
            }
            var stream = new CodedInputStream(rsp_string);
            var rsp = new DownloadResponse();
            try
            {
                rsp.MergeFrom(stream);
                stream.Dispose();
            }
            catch (System.Exception)
            {
                stream.Dispose();
                return data;
            }
            data.Version = rsp.Version;
            foreach (var file in rsp.File)
            {
                data.files.Add(file);
            }
            return data;
        }

        private static async Task<HttpResponseMessage> PostRequestAsync(HttpClient client, string base_url, Dictionary<String, String> kvs)
        {
            var query_url = HevoHelper.MakeQueryString(kvs);
            var gbk_query_url = HevoHelper.GbkEncoding.GetBytes(query_url);
            var post_content = new ByteArrayContent(gbk_query_url);
            post_content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
#if DEBUG
            post_content.Headers.Add("X-Arsenal-Auth", "futures-selfstock");
#endif

            var response_body = await client.PostAsync(base_url, post_content);
            return response_body;
        }


        public static async Task<CloudInfo> PushCloundData(HttpClient client, AuthInfo info, String biz, String path, Int64 version, String Filename, Byte[] file)
        {
            var kvs = new Dictionary<String, String>()
            {
                { "appname", biz },
                { "reqtype", "upload" },
                { "version", version.ToString() },
                { "storepath", path },
                { "clienttype", "hevo_pc"},
                { "compresstype", "none" },
                { "compresstype_upload", "none" },
                { "compresstype_download", "none" },
                { "userid", info?.UserID },
                { "sessionid", info?.SessionID},
                { "expires", info?.Expires},
                {"token", info?.ThirdSign}
            };
            var data = new CloudInfo();
            var header = await PostCloudFileAsync(client, BASE_URL, kvs, Filename, file);
            if (header == null)
            {
                data.Code = HttpStatusCode.NotFound;
                return data;
            }
            var rsp_string = await header.Content.ReadAsByteArrayAsync();

            data.Code = header.StatusCode;
            if (data.Code != HttpStatusCode.OK)
            {
                return data;
            }
            var stream = new CodedInputStream(rsp_string);
            var rsp = new UploadResponse();
            try
            {
                rsp.MergeFrom(stream);
                stream.Dispose();
            }
            catch (System.Exception)
            {
                stream.Dispose();
                return data;
            }
            data.Version = rsp.Version;
            return data;
        }

        private static async Task<HttpResponseMessage> PostCloudFileAsync(HttpClient client, string base_url, Dictionary<String, String> kvs, String file_name, Byte[] file)
        {
            var ticks = DateTime.Now.Ticks;
            String bondary = "----HevoFormBoundary";
            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                var ch = (Char)((random.Next(0, 20000) % 26) + (random.Next(0, 20000) % 2 > 0 ? 'a' : 'A'));
                bondary += ch;
            }
            try
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent(bondary))
                {
                    foreach (var data in kvs)
                    {
                        var post_content = new ByteArrayContent(HevoHelper.GbkEncoding.GetBytes(data.Value));
                        var value = String.Format("form-data;name=\"{0}\"", data.Key);
                        post_content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(value);
                        post_content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain;charset=US-ASCII");
                        post_content.Headers.ContentEncoding.Add("8bit");
                        content.Add(post_content);
                    }

                    var file_content = new ByteArrayContent(file);
                    var file_value = String.Format("form-data;name=\"uploadFile\";filename=\"{0}\"", file_name);
                    file_content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(file_value);
                    file_content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    file_content.Headers.ContentEncoding.Add("binary");
                    content.Add(file_content);
#if DEBUG
                    content.Headers.Add("x-Arsenal-Auth", "futures-selfstock");
#endif
                    var response_body = await client.PostAsync(base_url, content);

                    return response_body;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
            return null;
        }


        /// <summary>
        /// 内网 期货通App 源的自选股测试地址，需要在 Host 中配置 10.0.15.249 cloudstorage.cbas
        /// </summary>
        private static readonly string LocalTestCloudStorageUrl = "http://cloudstorage.cbas/multistorage";

        /// <summary>
        /// 获取云端App端自选股数据
        /// </summary>
        /// <param name="dataCenter"></param>
        /// <param name="info"></param>
        /// <param name="bizName"></param>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public static async Task<CloudInfo> DownloadSelfCodeDataAsync(
            HttpClient client,
            AuthInfo info,
            string bizName, string path, long version, string clientType)
        {
            var keyValues = new Dictionary<string, string>()
            {
                { "reqtype", "download" },
                { "userid", info.UserID },
                { "storepath", path },
                { "sessionid", info.SessionID },
                { "expires", info.Expires },
                { "token", info.ThirdSign },
                { "appname", bizName },
                { "clienttype", clientType },
                { "version", version.ToString() }
            };

#if DEBUG
            var httpResponseMessage = await PostRequestAsync(client, LocalTestCloudStorageUrl, keyValues);

#else
            var httpResponseMessage = await PostRequestAsync(client, BASE_URL, keyValues);
#endif

            if (httpResponseMessage == null)
            {
                return new CloudInfo { Code = HttpStatusCode.NotFound };
            }

            var responseString = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            var data = new CloudInfo
            {
                Code = httpResponseMessage.StatusCode
            };

            if (data.Code != HttpStatusCode.OK)
            {
                // 将非 200 的 HttpStatusCode 设为 500， version 为 0
                data.Code = HttpStatusCode.InternalServerError;

                return data;
            }
            var stream = new CodedInputStream(responseString);
            var rsp = new DownloadResponse();
            try
            {
                rsp.MergeFrom(stream);
                stream.Dispose();
            }
            catch (Exception)
            {
                stream.Dispose();
                data.Code = HttpStatusCode.InternalServerError;

                return data;
            }
            data.Version = rsp.Version;

            // DownloadResponse 的 Code 用来判断是否需要更新文件，以及用户文件的状态 404 表示 用户从未上传过 304 表示本地版本已经是最新 正常则返回
            // 200 兜底 HttpCode 500, Version 0
            if (rsp.Code == 200)
            {
                data.Code = HttpStatusCode.OK;
            }
            else if (rsp.Code == 304)
            {
                data.Code = HttpStatusCode.NotModified;
            }
            else if (rsp.Code == 404)
            {
                data.Code = HttpStatusCode.NotFound;
            }
            else
            {
                data.Code = HttpStatusCode.InternalServerError;
                data.Version = 0;

                return data;
            }

            foreach (var file in rsp.File)
            {
                data.files.Add(file);
            }

            return data;
        }

        /// <summary>
        /// 上传自选股数据至云端
        /// </summary>
        /// <param name="info"></param>
        /// <param name="bizName"></param>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <param name="file"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public static async Task<CloudInfo> PushSelfCodeToCloudAsync(
            HttpClient client,
            AuthInfo info,
            string bizName,
            string path,
            long version,
            byte[] file,
            string clientType)
        {
            string fileName = "futures_pc_selfcode";

            var keyValues = new Dictionary<string, string>()
            {
                { "appname", bizName },
                { "reqtype", "upload" },
                { "version", version.ToString() },
                { "storepath", path },
                { "clienttype", clientType },
                { "compresstype", "none" },
                { "compresstype_upload", "none" },
                { "compresstype_download", "none" },
                { "userid", info.UserID },
                { "sessionid", info.SessionID },
                { "expires", info.Expires },
                { "token", info.ThirdSign },
            };

            var data = new CloudInfo();

#if DEBUG
            var httpResponseMessage = await PostCloudFileAsync(client, LocalTestCloudStorageUrl, keyValues, fileName, file);

#else
            var httpResponseMessage = await PostCloudFileAsync(client, BASE_URL, keyValues, fileName, file);

#endif

            if (httpResponseMessage == null)
            {
                data.Code = HttpStatusCode.NotFound;
                return data;
            }
            var responseString = await httpResponseMessage.Content.ReadAsByteArrayAsync();

            data.Code = httpResponseMessage.StatusCode;
            if (data.Code != HttpStatusCode.OK)
            {
                return data;
            }
            var stream = new CodedInputStream(responseString);
            var rsp = new UploadResponse();
            try
            {
                rsp.MergeFrom(stream);
                stream.Dispose();
            }
            catch (Exception)
            {
                stream.Dispose();
                return data;
            }
            data.Version = rsp.Version;

            if (rsp.Code == -10002)
            {
                // 假设 response code 为 -10002，则置 data.Code 为 HttpStatusCode.ServiceUnavailable
                data.Code = HttpStatusCode.ServiceUnavailable;
                Debug.Print($"版本错误，目前版本号为{rsp.Version.ToString()}");
            }

            return data;
        }
    }
}
