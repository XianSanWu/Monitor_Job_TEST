using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.MailhunterLogParseRequest;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace WebApi.Controllers
{
    public partial class WorkflowStepsController : BaseController
    {
        /// <summary>
        /// 複製今日檔案 並刪除昨日檔案
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpPut("CopyTodayFilesAsync")]
        public async Task<ResultResponse<bool>> CopyTodayFilesAsync(
            string sourcePath = "", string targetPath = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(sourcePath))
                sourcePath = _config["FileCopySettings:MailhunterLogSourceDirectory"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(targetPath))
                targetPath = _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;

            var result = await _fileService.CopyTodayFilesAsync(jobGuid, sourcePath, targetPath, startsWithFileName, cancellationToken).ConfigureAwait(false);
            return SuccessResult(result);
        }

        #region 不要用，檔案過大字串會被截斷
        /// <summary>
        /// (不要用，檔案過大字串會被截斷) 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>每個檔案文字內容</returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpGet("GetTodayFileContentAsync")]
        public async Task<ResultResponse<string>> GetTodayFileContentAsync(
            string directory = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(directory))
                directory = _config["FileReadSettings:MailhunterLogDirectory"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;

            var result = await _fileService.GetTodayFileContentAsync(jobGuid, directory, startsWithFileName, cancellationToken).ConfigureAwait(false) ?? string.Empty;
            return SuccessResult(result);
        }

        /// <summary>
        /// (不要用，檔案過大字串會被截斷) 分段處理 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="chunkIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpGet("GetFileContentChunkAsync")]
        public async Task<ResultResponse<string>> GetFileContentChunkAsync(
            string directory = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(directory))
                directory = _config["FileReadSettings:MailhunterLogDirectory"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;

            string result = await _fileService.GetFileContentChunkAsync(jobGuid, directory, startsWithFileName, cancellationToken) ?? string.Empty;

            return SuccessResult(result);
        }
        #endregion


        /// <summary>
        /// 分段處理 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Action")]
        [HttpGet("GetFileContentChunkListAsync")]
        public async Task<ResultResponse<List<string>>> GetFileContentChunkListAsync(
            string directory = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(directory))
                directory = _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;

            var result = await _fileService.GetFileContentChunkListAsync(jobGuid, directory, startsWithFileName, cancellationToken).ConfigureAwait(false) ?? new List<string>();

            return SuccessResult(result);
        }

        /// <summary>
        /// 拆解文字檔，找出段落
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpPost("MailhunterLogParseLogAsync")]
        public async Task<ResultResponse<MailhunterLogParseResponse>> MailhunterLogParseLogAsync(MailhunterLogParseSearchListFieldModelRequest request, CancellationToken cancellationToken)
        {
            var jobGuid = Guid.NewGuid().ToString();

            var result = new MailhunterLogParseResponse();

            result = await _fileService.MailhunterLogParseLogAsync(jobGuid, request.LogContent ?? string.Empty, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
        }

        /// <summary>
        /// 拆解List文字檔，找出段落
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpPost("MailhunterLogParseLogListAsync")]
        public async Task<ResultResponse<MailhunterLogParseResponse>> MailhunterLogParseLogListAsync(MailhunterLogParseSearchListFieldModelRequest request, CancellationToken cancellationToken)
        {
            var jobGuid = Guid.NewGuid().ToString();

            var result = new MailhunterLogParseResponse();

            //var directory = _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? string.Empty;
            //var startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? string.Empty;

            //request.LogContentList = await _fileService.GetFileContentChunkListAsync(directory, startsWithFileName, _config, cancellationToken).ConfigureAwait(false) ?? new List<string>();

            result = await _fileService.MailhunterLogParseLogListAsync(jobGuid, request.LogContentList ?? new List<string>(), cancellationToken).ConfigureAwait(false);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查詢結果集合(多筆)
        /// </summary>
        /// <param name="searchReq">前端傳入的查詢條件</param>
        /// <param name="cancellationToken">取消非同步</param>
        /// <returns name="result">查詢結果 </returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpPost("SearchList")]
        public async Task<ResultResponse<WorkflowStepsSearchListResponse>> QuerySearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new WorkflowStepsSearchListResponse();
            #endregion

            #region 流程

            result = await _workflowStepsService.QueryWorkflowStepsSearchList(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }

        /// <summary>
        /// 工作流程資料更新(多筆)
        /// </summary>
        /// <param name="fieldReq"></param>
        /// <param name="conditionReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Action")]  //分組(可多標籤)        
        [HttpPost("UpdateWorkflowList")]
        public async Task<ResultResponse<bool>> UpdateWorkflowList(WorkflowUpdateListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = false;
            #endregion

            #region 流程
            var fieldReq = req.FieldReq;
            var conditionReq = req.ConditionReq;

            result = await _workflowStepsService.UpdateWorkflowList(fieldReq, conditionReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }


    }
}
