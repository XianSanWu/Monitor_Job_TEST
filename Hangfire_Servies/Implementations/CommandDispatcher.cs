using Hangfire_Models.Dto.Requests;
using Hangfire_Models.Enums;
using Hangfire_Servies.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Hangfire_Servies.Implementations
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IConfiguration _config;
        private readonly IApiService _apiService;

        #region config 參數
        private readonly string _scheme;
        private readonly string _host;
        private readonly string _port;
        private readonly string _basePath;
        private readonly string _path;
        #endregion

        public CommandDispatcher(
            ILogger<CommandDispatcher> logger,
            IConfiguration config,
            IApiService apiService)
        {
            _logger = logger;
            _config = config;
            _apiService = apiService;

            // 在建構子中初始化 config 參數
            _scheme = _config.GetValue<string>("ApiSettings:Scheme") ?? string.Empty;
            _host = _config.GetValue<string>("ApiSettings:Host") ?? string.Empty;
            _port = _config.GetValue<string>("ApiSettings:Port") ?? string.Empty;
            _basePath = _config.GetValue<string>("ApiSettings:BasePath") ?? string.Empty;
            _path = _config.GetValue<string>("ApiSettings:WorkflowSteps:UpdateWorkflowStatusJob") ?? string.Empty;
        }

        public async Task DispatchAsync(JobExecutionContext jobExecutionContext)
        {
            switch (jobExecutionContext.SelectMethod)
            {
                case nameof(ScheduleTypeEnum.UpdateWorkflowStatusJob):
                    string apiUrl = $"{_scheme}://{_host}{(!string.IsNullOrWhiteSpace(_port) ? $":{_port}" : "")}/{_basePath}/{_path}";

                    var result = await _apiService.CallApiAsync(
                        apiUrl: apiUrl,
                        method: HttpMethod.Post,
                        queryParams: null,         // 無 URL 參數
                        bodyContent: JsonSerializer.Serialize(jobExecutionContext)          // 無 POST Body
                    );

                    _logger.LogInformation("API 回傳內容: {Response}", result);
                    break;
                default:
                    _logger.LogWarning("無此排程選項");
                    break;
            }
        }
    }

}
