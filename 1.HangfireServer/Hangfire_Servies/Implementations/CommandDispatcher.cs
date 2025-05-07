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
        private readonly string _path_mailhunterJob;
        private readonly string _path_finishJob;
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
            _scheme = _config.GetValue<string>("ApiSettings:0:Scheme") ?? string.Empty;
            _host = _config.GetValue<string>("ApiSettings:0:Host") ?? string.Empty;
            _port = _config.GetValue<string>("ApiSettings:0:Port") ?? string.Empty;
            _basePath = _config.GetValue<string>("ApiSettings:0:BasePath") ?? string.Empty;
            _path_mailhunterJob = _config.GetValue<string>("ApiSettings:0:WorkflowSteps:UpdateWorkflowStatusMailhunterJob") ?? string.Empty;
            _path_finishJob = _config.GetValue<string>("ApiSettings:0:WorkflowSteps:UpdateWorkflowStatusFinishJob") ?? string.Empty;
        }

        public async Task DispatchAsync(JobExecutionContext jobExecutionContext)
        {
            string apiUrl = string.Empty;
            string result = string.Empty;

            switch (jobExecutionContext.SelectMethod)
            {
                #region 更新流程狀態任務>>Mailhunter
                case nameof(ScheduleTypeEnum.UpdateWorkflowStatusMailhunterJob):
                    apiUrl = $"{_scheme}://{_host}{(!string.IsNullOrWhiteSpace(_port) ? $":{_port}" : "")}/{_basePath}/{_path_mailhunterJob}";

                    result = await _apiService.CallApiAsync(
                        apiUrl: apiUrl,
                        method: HttpMethod.Post,
                        queryParams: null,         // 無 URL 參數
                        bodyContent: JsonSerializer.Serialize(jobExecutionContext)          // 無 POST Body
                    ).ConfigureAwait(false) ?? string.Empty;

                    _logger.LogInformation($"API：{apiUrl} 回傳內容：{result}");
                    break;
                #endregion

                #region 更新流程狀態任務>>Finish
                case nameof(ScheduleTypeEnum.UpdateWorkflowStatusFinishJob):
                    apiUrl = $"{_scheme}://{_host}{(!string.IsNullOrWhiteSpace(_port) ? $":{_port}" : "")}/{_basePath}/{_path_finishJob}";

                    result = await _apiService.CallApiAsync(
                        apiUrl: apiUrl,
                        method: HttpMethod.Post,
                        queryParams: null,         // 無 URL 參數
                        bodyContent: JsonSerializer.Serialize(jobExecutionContext)          // 無 POST Body
                    ).ConfigureAwait(false) ?? string.Empty;

                    _logger.LogInformation($"API：{apiUrl} 回傳內容：{result}");
                    break;
                #endregion

                default:
                    _logger.LogWarning("無此排程選項");
                    break;
            }
        }
    }

}
