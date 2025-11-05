using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Models.Common;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using System.Text.Json;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.AppMhProjectResponse.AppMhProjectSearchListResponse;

namespace WebApi.Controllers
{
    public partial class WorkflowStepsController : BaseController
    {
        #region Config 參數
        private string CopySourcePath => _config["FileCopySettings:MailhunterLogSourceDirectory"] ?? string.Empty;
        private string CopyTargetPath => _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? string.Empty;
        private string SendBatchFileName => _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;
        #endregion

        #region UpdateWorkflowStatusMailhunterJob
        [Tags("WorkflowSteps.Job")]
        [HttpPost("UpdateWorkflowStatusMailhunterJob")]
        public async Task<ResultResponse<bool>> UpdateWorkflowStatusMailhunterJob(JobExecutionContext jobExecutionContext, CancellationToken cancellationToken)
        {
            var jobGuid = $"JobExecutionContext：{JsonSerializer.Serialize(jobExecutionContext)}，JobGuid：{Guid.NewGuid()}";
            var result = false;

            try
            {
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusMailhunterJob 開始執行【{jobGuid}】", "", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusMailhunterJob Job 開始執行");

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
                var filterFileNameList = new List<FieldWithMetadataModel>
                {
                    new FieldWithMetadataModel
                    {
                        Key = "UploadFileName",
                        MathSymbol = MathSymbolEnum.In.ToString(),
                        Value = parsedLogs.CompletedJobs.ToArray()
                    }
                };


                filterFileNameList.Add(new FieldWithMetadataModel
                {
                    Key = "ProgressStatus",
                    MathSymbol = MathSymbolEnum.Equal.ToString(),
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
                    FilterModel = filterFileNameList
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

                #region Step 5: 合併已存在 + 解析後的檔名，做為更新目標
                var completedJobsSet = new HashSet<string>(parsedLogs.CompletedJobs);

                var mergedUploadFileNames = queryWfsList?.SearchItem
                    .Select(x => x.UploadFileName)
                    .Where(name => completedJobsSet.Contains(name ?? string.Empty))
                    .Distinct()
                    .ToList();

                if (mergedUploadFileNames?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】mergedUploadFileNames 沒有可更新資料");
                    return SuccessResult(result);
                }
                #endregion

                #region Step 6: 更新流程步驟狀態為 Mail_Hunter
                var fieldReq = new WorkflowStepsUpdateFieldRequest
                {
                    ProgressStatus = ProgressStatusTypeEnum.Mail_Hunter.ToString()
                };

                var conditionReq = new List<WorkflowStepsUpdateConditionRequest>
                {
                    new()
                    {
                        InsideLogicOperator = LogicOperatorEnum.AND,
                        GroupLogicOperator = LogicOperatorEnum.AND,
                        Channel = new FieldWithMetadataModel
                        {
                            MathSymbol = MathSymbolEnum.Equal.ToString(),
                            Value = ChannelTypeEnum.EDM.ToString()
                        },
                        UploadFileName = new FieldWithMetadataModel
                        {
                            MathSymbol = MathSymbolEnum.In.ToString(),
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
                    await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusMailhunterJob UpdateWorkflowList 更新失敗", $"請查看LOG：【{jobGuid}】", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                    return SuccessResult(result);
                }
                #endregion

                result = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"【{jobGuid}】UpdateWorkflowStatusMailhunterJob 執行失敗");
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusMailhunterJob 執行失敗【{jobGuid}】", $"ex：{ex}，exMsg：{ex?.Message}", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                throw new Exception($"【UpdateWorkflowStatusMailhunterJob】jobGuid：{jobGuid} 發生錯誤，EX：{ex}，EX_MSG：{ex?.Message}");
                //return SuccessResult(result);
            }
            finally
            {
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusMailhunterJob Job 執行結束");
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusMailhunterJob 執行結束【{jobGuid}】", "", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
            }

            return SuccessResult(result);
        }
        #endregion

        #region UpdateWorkflowStatusTodayFinishJob
        [Tags("WorkflowSteps.Job")]
        [HttpPost("UpdateWorkflowStatusTodayFinishJob")]
        public async Task<ResultResponse<bool>> UpdateWorkflowStatusTodayFinishJob(JobExecutionContext jobExecutionContext, CancellationToken cancellationToken)
        {
            var jobGuid = $"JobExecutionContext：{JsonSerializer.Serialize(jobExecutionContext)}，JobGuid：{Guid.NewGuid()}";
            var result = false;

            try
            {
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusTodayFinishJob 開始執行【{jobGuid}】", "", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusTodayFinishJob Job 開始執行");
                #region Step 1: 取得今日Mailhunter專案中有執行的BatchId
                List<AppMhProjectSearchResponse> appMhProjectList = await _mailhunterService.GetTodayAppMhProjectList(cancellationToken).ConfigureAwait(false);
                if (appMhProjectList?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】GetTodayAppMhProjectList 查詢為空");
                    return SuccessResult(result);
                }
                #endregion

                #region Step 2: 取得WorkflowSteps中，ProgressStatus=Mailhunter&&AccuCdpTotalCount>0的BatchId(去重)
                var filterFileNameList = new List<FieldWithMetadataModel>
                {
                    new() {
                        Key = "ProgressStatus",
                        MathSymbol = MathSymbolEnum.Equal.ToString(),
                        Value = ProgressStatusTypeEnum.Mail_Hunter.ToString()
                    },
                    new() {
                        Key = "AccuCdpTotalCount",
                        MathSymbol = MathSymbolEnum.GreaterThan.ToString(),
                        Value = 0
                    }

                };

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
                    FilterModel = filterFileNameList
                };

                var queryWfsList = await _workflowStepsService
                    .QueryWorkflowStepsSearchList(searchReq, cancellationToken)
                    .ConfigureAwait(false);

                if (queryWfsList?.SearchItem?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】QueryWorkflowStepsSearchList 查詢為空");
                    return SuccessResult(result);
                }

                var wfsBatchIdList = queryWfsList?.SearchItem.Select(x => x.BatchId).Distinct().ToList();
                #endregion

                #region Step 3: 合集取出要查詢資料
                var mergedProjectList = appMhProjectList?
                    .Where(p => wfsBatchIdList?.Contains(p.BatchNo.ToString()) == true)
                    .ToList();

                if (mergedProjectList?.Count == 0)
                {
                    _logger.LogWarning($"【{jobGuid}】mergedProjectList 沒有可查詢資料");
                    return SuccessResult(result);
                }
                #endregion

                var fieldReq = new WorkflowStepsUpdateFieldRequest();
                var conditionReq = new List<WorkflowStepsUpdateConditionRequest>();
                #region Step 4: 如果再app_mh_result中有成功的BatchId，請如果狀態為mailhunter並且成功率大於95%
                var highSuccessWfsItems = new List<string>();
                foreach (var project in mergedProjectList ?? [])
                {
                    var batchIdSuccess = await _mailhunterService.GetBatchIdAppMhResultSuccessCount(
                          new BatchIdAppMhResultSuccessCountRequest
                          {
                              ProjectId = project.ProjectId
                          }, cancellationToken).ConfigureAwait(false);

                    if (batchIdSuccess?.SuccessCount <= 0) continue;

                    //如果BatchId成功率大於95%
                    var highSuccessWfsItem =
                        queryWfsList?.SearchItem
                                     .FirstOrDefault(f =>
                                        f.BatchId == project.BatchNo.ToString() &&
                                        f.AccuCdpTotalCount > 0 &&
                                        (batchIdSuccess?.SuccessCount ?? 0) / (double)f.AccuCdpTotalCount > 0.95
                                     );

                    if (highSuccessWfsItem is null) continue;

                    #region Step 5: 更新WorkflowSteps的BatchId最新一筆MailhunterSuccessCount數量
                    fieldReq = new WorkflowStepsUpdateFieldRequest
                    {
                        MailhunterSuccessCount = batchIdSuccess?.SuccessCount,
                    };

                    conditionReq =
                    [
                        new()
                        {
                            InsideLogicOperator = LogicOperatorEnum.AND,
                            GroupLogicOperator = LogicOperatorEnum.AND,
                            Channel = new FieldWithMetadataModel
                            {
                                MathSymbol = MathSymbolEnum.Equal.ToString(),
                                Value = ChannelTypeEnum.EDM.ToString()
                            },
                            BatchId = new FieldWithMetadataModel
                            {
                                MathSymbol = MathSymbolEnum.Equal.ToString(),
                                Value = project.BatchNo.ToString()
                            },
                            CreateAt = new FieldWithMetadataModel
                            {
                                MathSymbol = MathSymbolEnum.Max.ToString(),
                            },
                        }
                    ];

                    var updateWfsList = await _workflowStepsService
                        .UpdateWorkflowList(fieldReq, conditionReq, cancellationToken)
                        .ConfigureAwait(false);

                    if (!updateWfsList)
                    {
                        _logger.LogWarning($"【{jobGuid}】UpdateWorkflowList 更新失敗，更新WorkflowSteps的BatchId最新一筆MailhunterSuccessCount數量");
                        await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusTodayFinishJob UpdateWorkflowList 更新失敗", $"請查看LOG：【{jobGuid}】更新WorkflowSteps的BatchId最新一筆MailhunterSuccessCount數量", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                        return SuccessResult(result);
                    }
                    #endregion

                    highSuccessWfsItems.Add(project.BatchNo?.ToString() ?? "");
                }
                #endregion

                fieldReq = new WorkflowStepsUpdateFieldRequest();
                conditionReq = new List<WorkflowStepsUpdateConditionRequest>();
                #region 更新WorkflowSteps的狀態為Finish
                fieldReq = new WorkflowStepsUpdateFieldRequest
                {
                    ProgressStatus = ProgressStatusTypeEnum.Finish.ToString(),
                };

                conditionReq =
                [
                    new()
                    {
                        InsideLogicOperator = LogicOperatorEnum.AND,
                        GroupLogicOperator = LogicOperatorEnum.AND,
                        Channel = new FieldWithMetadataModel
                        {
                            MathSymbol = MathSymbolEnum.Equal.ToString(),
                            Value = ChannelTypeEnum.EDM.ToString()
                        },
                        BatchId = new FieldWithMetadataModel
                        {
                            MathSymbol = MathSymbolEnum.In.ToString(),
                            Value = highSuccessWfsItems
                        }
                    }
                ];

                var updateWfsFinishList = await _workflowStepsService
                    .UpdateWorkflowList(fieldReq, conditionReq, cancellationToken)
                    .ConfigureAwait(false);

                if (!updateWfsFinishList)
                {
                    _logger.LogWarning($"【{jobGuid}】UpdateWorkflowList 更新失敗，更新WorkflowSteps的狀態為Finish");
                    await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusTodayFinishJob UpdateWorkflowList 更新失敗", $"請查看LOG：【{jobGuid}】更新WorkflowSteps的狀態為Finish", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                    return SuccessResult(result);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"【{jobGuid}】UpdateWorkflowStatusTodayFinishJob 執行失敗");
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusTodayFinishJob 執行失敗【{jobGuid}】", $"ex：{ex}，exMsg：{ex?.Message}", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
                throw new Exception($"【UpdateWorkflowStatusTodayFinishJob】jobGuid：{jobGuid} 發生錯誤，EX：{ex}，EX_MSG：{ex?.Message}");
                //return SuccessResult(result);
            }
            finally
            {
                _logger.LogInformation($"【{jobGuid}】UpdateWorkflowStatusTodayFinishJob Job 執行結束");
                await _mailService.SendMailAndColineAsync($"UpdateWorkflowStatusTodayFinishJob 執行結束【{jobGuid}】", "", "", "", true, $"【{jobGuid}】").ConfigureAwait(false);
            }
            result = true;

            return SuccessResult(result);

        }
        #endregion
    }
}
