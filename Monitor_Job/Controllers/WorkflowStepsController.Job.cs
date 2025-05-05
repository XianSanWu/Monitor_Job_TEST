using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Common;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Threading.Channels;
using static Models.Dto.Requests.WorkflowStepsRequest;

namespace WebApi.Controllers
{
    public partial class WorkflowStepsController : BaseController
    {
        #region Config 參數
        private string CopySourcePath => _config["FileCopySettings:MailhunterLogSourceDirectory"] ?? string.Empty;
        private string CopyTargetPath => _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? string.Empty;
        private string SendBatchFileName => _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;
        #endregion

        [Tags("WorkflowSteps.Job")]
        [HttpPost("UpdateWorkflowStatusJob")]
        public async Task<ResultResponse<bool>> UpdateWorkflowStatusJob(JobExecutionContext jobExecutionContext, CancellationToken cancellationToken)
        {
            var jobGuid = $"JobExecutionContext：{JsonSerializer.Serialize(jobExecutionContext)}，JobGuid：{Guid.NewGuid()}";
            var result = false;

            try
            {
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusJob 開始執行【{jobGuid}】", "", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusJob Job 開始執行");

                #region Step 1: 複製當日檔案
                var copySuccess = await _fileService.CopyTodayFilesAsync(jobGuid, CopySourcePath, CopyTargetPath, SendBatchFileName, cancellationToken).ConfigureAwait(false);
                if (!copySuccess)
                {
                    _logger.LogWarning($"【{jobGuid}】CopyTodayFilesAsync 失敗，來源: {CopySourcePath}，目標: {CopyTargetPath}");
                    return SuccessResult(result);
                }
                #endregion

                #region Step 2: 讀取檔案內容（切塊）
                var chunks = await _fileService.GetFileContentChunkListAsync(jobGuid, CopyTargetPath, SendBatchFileName, cancellationToken).ConfigureAwait(false);
                if (chunks?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】GetFileContentChunkListAsync 回傳空，路徑: {CopyTargetPath}，檔名: {SendBatchFileName}");
                    return SuccessResult(result);
                }
                #endregion

                #region Step 3: 解析檔案內容
                var parsedLogs = await _fileService.MailhunterLogParseLogListAsync(jobGuid, chunks ?? [], cancellationToken).ConfigureAwait(false);

                // 若解析失敗或無 CompletedJobs，回傳失敗
                if (parsedLogs == null || parsedLogs.CompletedJobs == null || parsedLogs.CompletedJobs.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】MailhunterLogParseLogListAsync 無法解析 chunk list 或無 CompletedJobs");
                    return SuccessResult(result);
                }

                // 移除重複項目
                parsedLogs.CompletedJobs = parsedLogs.CompletedJobs.Distinct().ToList();
                #endregion

                #region Step 4: 查詢對應的流程步驟
                var filterUploadFileNameList = parsedLogs.CompletedJobs
                    .Select(job => new Option
                    {
                        Key = "UploadFileName",
                        Value = job
                    })
                    .ToList();

                filterUploadFileNameList.Add(new Option
                {
                    Key = "ProgressStatus",
                    Value = ProgressStatusTypeEnum.FTP.ToString()
                });

                var searchReq = new WorkflowStepsSearchListRequest
                {
                    Page = new PageBase
                    {
                        PageIndex = 1,
                        PageSize = int.MaxValue
                    },
                    FieldModel = new WorkflowStepsSearchListFieldModelRequest
                    {
                        Channel = ChannelTypeEnum.EDM.ToString()
                    },
                    FilterModel = filterUploadFileNameList
                };

                var queryWfsList = await _workflowStepsService
                    .QueryWorkflowStepsSearchList(searchReq, cancellationToken)
                    .ConfigureAwait(false);

                if (queryWfsList?.SearchItem?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】QueryWorkflowStepsSearchList 查詢為空");
                    return SuccessResult(result);
                }
                #endregion

                #region 合併已存在 + 解析後的檔名，做為更新目標
                var mergedUploadFileNames = queryWfsList?.SearchItem
                    .Select(x => x.UploadFileName)
                    .Concat(parsedLogs.CompletedJobs)
                    .Distinct()
                    .ToList();
                #endregion

                #region Step 5: 更新流程步驟狀態為 Mail_Hunter
                var fieldReq = new WorkflowStepsUpdateFieldRequest
                {
                    ProgressStatus = ProgressStatusTypeEnum.Mail_Hunter.ToString()
                };

                var conditionReq = new List<WorkflowStepsUpdateConditionRequest>
                {
                    new()
                    {
                        Channel = new FieldWithMetadataModel
                        {
                            MathSymbol = MathSymbolEnum.Equal,
                            Value = ChannelTypeEnum.EDM.ToString()
                        },
                        UploadFileName = new FieldWithMetadataModel
                        {
                            MathSymbol = MathSymbolEnum.In,
                            Value = mergedUploadFileNames
                        }
                    }
                };

                var updateWfsList = await _workflowStepsService
                    .UpdateWorkflowList(fieldReq, conditionReq, cancellationToken)
                    .ConfigureAwait(false);

                if (!updateWfsList)
                {
                    _logger.LogWarning($"【{jobGuid}】UpdateWorkflowList 更新失敗");
                    await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusJob UpdateWorkflowList 更新失敗", $"請查看LOG：【{jobGuid}】", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                    return SuccessResult(result);
                }

                result = true;
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"【{jobGuid}】UpdateWorkflowStatusJob 執行失敗");
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusJob 執行失敗【{jobGuid}】", $"ex：{ex}，exMsg：{ex?.Message}", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                throw new Exception($"【UpdateWorkflowStatusJob】jobGuid：{jobGuid} 發生錯誤，EX：{ex}，EX_MSG：{ex?.Message}");
                //return SuccessResult(result);
            }
            finally
            {
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusJob Job 執行結束");
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusJob 執行結束【{jobGuid}】", "", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
            }

            return SuccessResult(result);
        }
    }
}
