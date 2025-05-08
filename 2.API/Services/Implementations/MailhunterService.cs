using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using Repository.Implementations;
using Repository.Implementations.MailhunterRespository;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class MailhunterService(ILogger<MailhunterService> logger, IConfiguration config, IMapper mapper) : IMailhunterService
    {
        private readonly ILogger<MailhunterService> _logger = logger;
        private readonly IConfiguration _config = config;

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
            var MHU_dbHelper = new DbHelper(_config, DBConnectionEnum.Mail_hunter);
#if TEST
            MHU_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = MHU_dbHelper)
            {
                IMailhunterRespository _mhuRp = new MailhunterRespository(dbHelper.UnitOfWork, mapper);
                result = await _mhuRp.GetTodayAppMhProjectList(cancellationToken).ConfigureAwait(false);
            }

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
            var MHU_dbHelper = new DbHelper(_config, DBConnectionEnum.Mail_hunter);
#if TEST
            MHU_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = MHU_dbHelper)
            {
                IMailhunterRespository _mhuRp = new MailhunterRespository(dbHelper.UnitOfWork, mapper);
                result = await _mhuRp.GetBatchIdAppMhResultSuccessCount(req, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }
    }
}
