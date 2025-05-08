using Models.Dto.Requests;
using Models.Dto.Responses;

namespace Services.Interfaces
{
    public interface IMailhunterService
    {
        /// <summary>
        /// 查詢當天的專案資料(多筆)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<AppMhProjectResponse>> GetTodayAppMhProjectList(CancellationToken cancellationToken);

        /// <summary>
        /// 查詢BatchId成功數量(單筆)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BatchIdAppMhResultSuccessCountResponse> GetBatchIdAppMhResultSuccessCount(BatchIdAppMhResultSuccessCountRequest req, CancellationToken cancellationToken);

    }
}
