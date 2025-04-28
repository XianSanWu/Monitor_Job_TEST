using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Services.Interfaces
{
    public interface IWorkflowStepsService
    {
        /// <summary>
        /// 工作進度查詢
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 工作流程資料更新(多筆)
        /// </summary>
        /// <param name="fieldReq"></param>
        /// <param name="conditionReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateWorkflowList(WorkflowStepsUpdateFieldRequest fieldReq, List<WorkflowStepsUpdateConditionRequest> conditionReq, CancellationToken cancellationToken);
    }
}
