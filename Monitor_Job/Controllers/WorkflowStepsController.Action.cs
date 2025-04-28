using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace JobScheduling.Controllers
{
    public partial class WorkflowStepsController : BaseController
    {
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
        public async Task<ResultResponse<bool>> UpdateWorkflowList(UpdateWorkflowListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = false;
            #endregion

            #region 流程
            var fieldReq = req.FieldReq;
            var conditionReq = req.ConditionReq;

            result = await _workflowStepsService.UpdateWorkflowList(fieldReq, conditionReq, _config, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }


    }
}
