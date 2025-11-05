using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;
using static Models.Entities.Responses.WorkflowEntityResponse;

namespace Repository.Interfaces
{
    public interface IWorkflowStepsRespository : IRepository
    {
        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsEntitySearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListEntityRequest searchReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 工作流程資料更新(多筆)DB
        /// </summary>
        /// <param name="fieldReq"></param>
        /// <param name="conditionReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateWorkflowList(WorkflowStepsUpdateFieldEntityRequest fieldReq, List<WorkflowStepsUpdateConditionEntityRequest> conditionReq, CancellationToken cancellationToken);
    }
}
