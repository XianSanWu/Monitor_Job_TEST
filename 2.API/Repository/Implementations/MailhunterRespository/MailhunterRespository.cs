using AutoMapper;
using Dapper;
using Models.Entities.Requests;
using Repository.Interfaces;
using static Models.Entities.Responses.AppMhProjectEntityResponse;
using static Models.Entities.Responses.BatchIdAppMhResultSuccessCountEntityResponse;

namespace Repository.Implementations.MailhunterRespository
{
    public partial class MailhunterRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), IMailhunterRespository, IRepository
    {
        /// <summary>
        /// 查詢當天的專案資料(多筆)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<AppMhProjectEntity>> GetTodayAppMhProjectList(CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<AppMhProjectEntity>();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryTodayAppMhProject();

            var queryEntity = (await _unitOfWork.Connection.QueryAsync<AppMhProjectEntity>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false)).ToList();
            result = _mapper.Map<List<AppMhProjectEntity>>(queryEntity);

            return result;

            #endregion
        }

        /// <summary>
        /// 查詢BatchId成功數量(多筆)DB
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BatchIdAppMhResultSuccessCountEntity> GetBatchIdAppMhResultSuccessCount(BatchIdAppMhResultSuccessCountEntityRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new BatchIdAppMhResultSuccessCountEntity();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryBatchIdAppMhResultSuccessCount(req);

            var queryEntity = (await _unitOfWork.Connection.QueryAsync<BatchIdAppMhResultSuccessCountEntity>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false)).ToList();
            result = _mapper.Map<BatchIdAppMhResultSuccessCountEntity>(queryEntity);

            return result;

            #endregion
        }

    }
}
