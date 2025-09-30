using Models.Dto.Requests;
using Models.Dto.Responses;

namespace Repository.Interfaces
{
    public interface IMailhunterRespository : IRepository
    {
        /// <summary>
        /// 查詢當天的專案資料(多筆)DB
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<AppMhProjectResponse>> GetTodayAppMhProjectList(CancellationToken cancellationToken);

        /// <summary>
        /// 查詢BatchId成功數量(單筆)DB
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BatchIdAppMhResultSuccessCountResponse> GetBatchIdAppMhResultSuccessCount(BatchIdAppMhResultSuccessCountRequest req, CancellationToken cancellationToken);

    }
}
