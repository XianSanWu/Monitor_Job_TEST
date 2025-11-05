using Microsoft.AspNetCore.Mvc;
using Models.Dto.Requests;
using Models.Dto.Responses;
using static Models.Dto.Responses.AppMhProjectResponse.AppMhProjectSearchListResponse;
using static Models.Dto.Responses.BatchIdAppMhResultSuccessCountResponse.BatchIdAppMhResultSuccessCountSearchListResponse;

namespace WebApi.Controllers
{
    public partial class MailhunterController : BaseController
    {

        /// <summary>
        /// 查詢當天的專案資料(多筆)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Mailhunter.Action")]  //分組(可多標籤)        
        [HttpPost("GetTodayAppMhProjectList")]
        public async Task<ResultResponse<List<AppMhProjectSearchResponse>>> GetTodayAppMhProjectList(CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new List<AppMhProjectSearchResponse>();
            #endregion

            #region 流程

            result = await _mailhunterService.GetTodayAppMhProjectList(cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion
        }

        /// <summary>
        /// 查詢BatchId成功數量(多筆)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Mailhunter.Action")]  //分組(可多標籤)        
        [HttpPost("GetBatchIdAppMhResultSuccessCount")]
        public async Task<ResultResponse<BatchIdAppMhResultSuccessCountSearchResponse>> GetBatchIdAppMhResultSuccessCount(BatchIdAppMhResultSuccessCountRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new BatchIdAppMhResultSuccessCountSearchResponse();
            #endregion

            #region 流程

            result = await _mailhunterService.GetBatchIdAppMhResultSuccessCount(req, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion
        }

    }
}
