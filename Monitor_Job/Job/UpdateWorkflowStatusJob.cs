using Hangfire;
using Hangfire.Dashboard.Management.v2.Metadata;
using Services.Interfaces;
using Services.Job.Interfaces;
using System.ComponentModel;

namespace JobScheduling.Job
{
    [ManagementPage(MenuName = "Update Workflow Status Job", Title = "更新工作流程狀態")]
    public class UpdateWorkflowStatusJob(
        IFileService fileService,
        ILogger<UpdateWorkflowStatusJob> logger,
        IConfiguration config
    ) : IRecurringJob
    {
        private readonly IFileService _fileService = fileService;
        private readonly ILogger<UpdateWorkflowStatusJob> _logger = logger;
        private readonly IConfiguration _config = config;

        public string JobId => "sync-mailhunter-update-workflow-status-job";
        public string CronExpression => _config["JobSchedulesCron:sync-mailhunter-update-workflow-status-job"] ?? "";

        #region Config 參數
        private string CopySourcePath => _config["FileCopySettings:MailhunterLogSourceDirectory"] ?? string.Empty;
        private string CopyTargetPath => _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? string.Empty;
        private string SendBatchFileName => _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;
        #endregion

        [Description("Mail hunter更新工作流程狀態")]
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var jobGuid = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusJob Job 開始執行");

                // Step 1: Copy today's file
                var copySuccess = await _fileService.CopyTodayFilesAsync(jobGuid, CopySourcePath, CopyTargetPath, SendBatchFileName, _config, cancellationToken).ConfigureAwait(false);
                if (!copySuccess)
                {
                    _logger.LogWarning($"【{jobGuid}】CopyTodayFilesAsync 失敗，來源: {CopySourcePath}，目標: {CopyTargetPath}");
                    return;
                }

                // Step 2: Read content
                var chunks = await _fileService.GetFileContentChunkListAsync(jobGuid, CopyTargetPath, SendBatchFileName, _config, cancellationToken).ConfigureAwait(false);
                if (chunks is null || chunks?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】GetFileContentChunkListAsync 回傳空，路徑: {CopyTargetPath}，檔名: {SendBatchFileName}");
                    return;
                }

                // Step 3: Parse content
                var parsedLogs = await _fileService.MailhunterLogParseLogListAsync(jobGuid, chunks ?? [], _config, cancellationToken).ConfigureAwait(false);
                if (parsedLogs is null || parsedLogs.CompletedJobs.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】MailhunterLogParseLogListAsync 無法解析 chunk list");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"【{jobGuid}】UpdateWorkflowStatusJob 執行失敗");
            }
            finally
            {
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusJob Job 執行結束");
            }

        }
    }
}
