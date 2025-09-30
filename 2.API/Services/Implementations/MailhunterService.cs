using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using Repository.Interfaces;
using Services.Interfaces;
using Repository.UnitOfWorkExtension;

namespace Services.Implementations
{
    public class MailhunterService(
        ILogger<MailhunterService> logger, 
        IConfiguration config,
        IMapper mapper,
        IUnitOfWorkFactory uowFactory,
        IRepositoryFactory repositoryFactory,
        IUnitOfWorkScopeAccessor scopeAccessor
        ) : IMailhunterService
    {
        private readonly ILogger<MailhunterService> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        /// <summary>
        /// 查詢當天的專案資料(多筆)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<AppMhProjectResponse>> GetTodayAppMhProjectList(CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new List<AppMhProjectResponse>();
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Mail_hunter;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IMailhunterRespository>(_scopeAccessor);

            result = await repo.GetTodayAppMhProjectList(cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 查詢BatchId成功數量(多筆)
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
            var dbType = DBConnectionEnum.Mail_hunter;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IMailhunterRespository>(_scopeAccessor);

            result = await repo.GetBatchIdAppMhResultSuccessCount(req, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }
    }
}
