using AutoMapper;
using Dapper;
using Models.Dto.Responses;
using Models.Entities;
using Repository.Interfaces;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;
using static Models.Dto.Responses.WorkflowStepsResponse;
using Models.Dto.Requests;

namespace Repository.Implementations.MailhunterRespository
{
    public partial class MailhunterRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), IMailhunterRespository
    {
        /// <summary>
        /// 查詢當天的專案資料(多筆)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<AppMhProjectResponse>> GetTodayAppMhProjectList(CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<AppMhProjectResponse>();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryTodayAppMhProject();

            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<AppMhProjectEntity>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false)).ToList();
            result = _mapper.Map<List<AppMhProjectResponse>>(queryWorkflowEntity);

            return result;

            #endregion
        }

        /// <summary>
        /// 查詢BatchId成功數量(多筆)DB
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BatchIdAppMhResultSuccessCountResponse> GetBatchIdAppMhResultSuccessCount(BatchIdAppMhResultSuccessCountRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new BatchIdAppMhResultSuccessCountResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryBatchIdAppMhResultSuccessCount(req);

            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<BatchIdAppMhResultSuccessCountEntity>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false)).ToList();
            result = _mapper.Map<BatchIdAppMhResultSuccessCountResponse>(queryWorkflowEntity);

            return result;

            #endregion
        }

    }
}
