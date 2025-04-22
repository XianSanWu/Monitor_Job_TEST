
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Requests;
using Models.Dto.Responses;
using System.IO;
using System.Text;
using YamlDotNet.Core.Tokens;
namespace JobScheduling.Controllers
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
        [Tags("WorkflowSteps.Job")]  //分組(可多標籤)        
        [HttpPut("CopyTodayFilesAsync")]
        public async Task<ResultResponse<bool>> CopyTodayFilesAsync(
            string sourcePath = "", string targetPath = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(sourcePath))
                sourcePath = _config["FileCopySettings:MailhunterLogSourceDirectory"] ?? "";

            if (string.IsNullOrWhiteSpace(targetPath))
                targetPath = _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? "";

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? "";

            var result = await _fileService.CopyTodayFilesAsync(jobGuid, sourcePath, targetPath, startsWithFileName, _config, cancellationToken).ConfigureAwait(false);
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
        [Tags("WorkflowSteps.Job")]  //分組(可多標籤)        
        [HttpGet("GetTodayFileContentAsync")]
        public async Task<ResultResponse<string>> GetTodayFileContentAsync(
            string directory = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(directory))
                directory = _config["FileReadSettings:MailhunterLogDirectory"] ?? "";

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? "";

            var result = await _fileService.GetTodayFileContentAsync(jobGuid, directory, startsWithFileName, _config, cancellationToken).ConfigureAwait(false) ?? "";
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
        [Tags("WorkflowSteps.Job")]  //分組(可多標籤)        
        [HttpGet("GetFileContentChunkAsync")]
        public async Task<ResultResponse<string>> GetFileContentChunkAsync(
            string directory = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(directory))
                directory = _config["FileReadSettings:MailhunterLogDirectory"] ?? "";

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? "";

            string result = await _fileService.GetFileContentChunkAsync(jobGuid, directory, startsWithFileName, _config, cancellationToken) ?? "";

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
        [Tags("WorkflowSteps.Job")]
        [HttpGet("GetFileContentChunkListAsync")]
        public async Task<ResultResponse<List<string>>> GetFileContentChunkListAsync(
            string directory = "", string startsWithFileName = "",
            CancellationToken cancellationToken = default
            )
        {
            var jobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(directory))
                directory = _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? "";

            if (string.IsNullOrWhiteSpace(startsWithFileName))
                startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? "";

            var result = await _fileService.GetFileContentChunkListAsync(jobGuid, directory, startsWithFileName, _config, cancellationToken).ConfigureAwait(false) ?? new List<string>();

            return SuccessResult(result);
        }

        /// <summary>
        /// 拆解文字檔，找出段落
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Job")]  //分組(可多標籤)        
        [HttpPost("MailhunterLogParseLogAsync")]
        public async Task<ResultResponse<MailhunterLogParseResponse>> MailhunterLogParseLogAsync(MailhunterLogParseRequest request, CancellationToken cancellationToken)
        {
            var jobGuid = Guid.NewGuid().ToString();

            var result = new MailhunterLogParseResponse();

            result = await _fileService.MailhunterLogParseLogAsync(jobGuid, request.LogContent ?? "", _config, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
        }

        /// <summary>
        /// 拆解List文字檔，找出段落
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps.Job")]  //分組(可多標籤)        
        [HttpPost("MailhunterLogParseLogListAsync")]
        public async Task<ResultResponse<MailhunterLogParseResponse>> MailhunterLogParseLogListAsync(MailhunterLogParseRequest request, CancellationToken cancellationToken)
        {
            var jobGuid = Guid.NewGuid().ToString();

            var result = new MailhunterLogParseResponse();

            //var directory = _config["FileCopySettings:MailhunterLogTargetDirectory"] ?? "";
            //var startsWithFileName = _config["FileReadFileNameSettings:FeibSendBatch"] ?? "";

            //request.LogContentList = await _fileService.GetFileContentChunkListAsync(directory, startsWithFileName, _config, cancellationToken).ConfigureAwait(false) ?? new List<string>();

            result = await _fileService.MailhunterLogParseLogListAsync(jobGuid, request.LogContentList ?? new List<string>(), _config, cancellationToken).ConfigureAwait(false);
            return SuccessResult(result);
        }

    }
}
