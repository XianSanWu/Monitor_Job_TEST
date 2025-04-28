using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Enums;
using Repository.Implementations;
using Repository.Implementations.WorkflowStepsRespository;
using Repository.Interfaces;
using Services.Interfaces;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Services.Implementations
{
    public class WorkflowStepsService(
        ILogger<WorkflowStepsService> logger,
        IConfiguration config,
        IMapper mapper) : IWorkflowStepsService
    {
        private readonly ILogger<WorkflowStepsService> _logger = logger;
        private readonly IConfiguration _config = config;

        /// <summary>
        /// 工作進度查詢
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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if DEBUG
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IWorkflowStepsRespository _wfsRp = new WorkflowStepsRespository(dbHelper.UnitOfWork, mapper);
                result = await _wfsRp.QueryWorkflowStepsSearchList(searchReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 工作流程資料更新(多筆)
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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if DEBUG
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IWorkflowStepsRespository _wfsRp = new WorkflowStepsRespository(dbHelper.UnitOfWork, mapper);
                result = await _wfsRp.UpdateWorkflowList(fieldReq, conditionReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

    }
}
