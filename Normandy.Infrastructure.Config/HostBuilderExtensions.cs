using Microsoft.Extensions.Hosting;

namespace Normandy.Infrastructure.Config
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// 添加Apollo 配置
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// 读取示例：
        /// var config = app.Services.GetRequiredService<IConfiguration>();
        /// var timeout = config.GetSection("Timeout");
        /// var port = config["host"];
        /// var zip = config["application/zip:0"];
        public static IHostBuilder AddApolloConfig(this IHostBuilder hostBuilder, string key = "apollo") =>
                hostBuilder.AddApollo(true, key);
    }
}
