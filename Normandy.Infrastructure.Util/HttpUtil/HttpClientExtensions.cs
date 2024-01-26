using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Util.HttpUtil
{
    /// <summary>
    /// http扩展方法
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// 请求
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<TResponse> RequestAsync<TResponse>(
            this HttpClient client, 
            HttpRequestMessage message)
        {
            var response = await client.SendAsync(message);
            var responseContent = await response.Content?.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {               
                if(string.IsNullOrWhiteSpace(responseContent))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });               
            }

            throw new HttpRequestException($"{response.StatusCode} ResponseContent: {responseContent})");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <param name="afterAction"></param>
        /// <param name="responseFailed"></param>
        /// <param name="beforeEvent"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<byte[]> RequestAsync(
            this HttpClient client, 
            HttpRequestMessage message)
        {
            var response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                var responseByteArray = await response.Content.ReadAsByteArrayAsync();
                return responseByteArray;
            }

            throw new HttpRequestException($"{response.StatusCode} (ReasonPhrase: {response.ReasonPhrase}");
        }
    }
}
