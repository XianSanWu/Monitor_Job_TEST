using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Enums;
using Repository.Interfaces;
using Repository.UnitOfWorkExtension;
using Services.Interfaces;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;

namespace Services.Implementations
{
    public class WorkflowStepsService(
        ILogger<WorkflowStepsService> logger,
        IConfiguration config,
        IMapper mapper,
        IUnitOfWorkFactory uowFactory,
        IRepositoryFactory repositoryFactory,
        IUnitOfWorkScopeAccessor scopeAccessor
        ) : IWorkflowStepsService
    {
        private readonly ILogger<WorkflowStepsService> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        /// <summary>
        /// 工作進度查詢
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest req, CancellationToken cancellationToken = default)
        {
            var entityReq = mapper.Map<WorkflowStepsSearchListEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IWorkflowStepsRespository>(_scopeAccessor);
            var entityResp = await repo.QueryWorkflowStepsSearchList(entityReq, cancellationToken);
            var result = mapper.Map<WorkflowStepsSearchListResponse>(entityResp);
            
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
            var entityFieldReq = mapper.Map<WorkflowStepsUpdateFieldEntityRequest>(fieldReq);
            var entityConditionReq = mapper.Map<List<WorkflowStepsUpdateConditionEntityRequest>>(conditionReq);

            #region 參數宣告
            var result = false;
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IWorkflowStepsRespository>(_scopeAccessor);

            result = await repo.UpdateWorkflowList(entityFieldReq, entityConditionReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

    }
}
