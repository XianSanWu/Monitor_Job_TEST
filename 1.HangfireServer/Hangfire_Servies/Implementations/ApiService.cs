using Hangfire_Servies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire_Servies.Implementations
{

    public class ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger) : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<ApiService> _logger = logger;

        public async Task<string?> CallApiAsync(string apiUrl, HttpMethod method, Dictionary<string, string>? queryParams = null, string? bodyContent = null)
        {
            var client = _httpClientFactory.CreateClient();

            // 加入 URL 參數
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                apiUrl += apiUrl.Contains("?") ? "&" + query : "?" + query;
            }

            try
            {
                var requestMessage = new HttpRequestMessage(method, apiUrl);

                if (bodyContent != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    requestMessage.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(requestMessage).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    _logger.LogError("API 呼叫失敗，狀態碼：{StatusCode}", response.StatusCode);
                    throw new Exception($"API 呼叫失敗，API_URL：{apiUrl}，狀態碼：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "呼叫 API 發生錯誤。");
                throw new Exception($"API 呼叫失敗，API_URL：{apiUrl}，EX：{ex}，EX_MSG：{ex?.Message}");
            }
        }
    }

}
