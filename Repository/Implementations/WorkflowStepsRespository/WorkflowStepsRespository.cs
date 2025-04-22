using AutoMapper;
using Dapper;
using Models.Entities;
using Repository.Interfaces;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), IWorkflowStepsRespository
    {
        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsSearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryWorkflowSql(searchReq);

            result.Page = searchReq.Page;
            result.SearchItem = new List<WorkflowStepsSearchResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<WorkflowEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<WorkflowStepsSearchResponse>>(queryWorkflowEntity);
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }

        /// <summary>
        /// 工作流程資料更新(多筆)DB
        /// </summary>
        /// <param name="fieldReq"></param>
        /// <param name="conditionReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateWorkflowList(WorkflowStepsUpdateFieldRequest fieldReq, List<WorkflowStepsUpdateConditionRequest> conditionReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            UpdateWorkflowSql(fieldReq, conditionReq);

            // 使用 ExecuteAsync 來執行 SQL 更新，並傳入 cancellationToken
            var affectedRows = await _unitOfWork.Connection.ExecuteAsync(_sqlStr?.ToString() ?? "", _sqlParams).ConfigureAwait(false);

            // 判斷是否有資料被更新
            result = affectedRows > 0;

            return result;

            #endregion
        }

    }

}
