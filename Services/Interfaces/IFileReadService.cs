using Microsoft.Extensions.Configuration;
using Models.Dto.Responses;

namespace Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        ///1. 從資料夾 A 複製今日的檔案到資料夾 B
        ///2. 只複製名稱開頭符合指定字串(StartsWith)
        ///3. 若資料夾 B 不存在則建立
        ///4. 如果今天過了，刪除 B 資料夾內的昨日檔案
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CopyTodayFilesAsync(string jobGuid, string sourcePath, string targetPath, string startsWithFileName, IConfiguration _config, CancellationToken cancellationToken);

        #region 不要用，檔案過大字串會被截斷
        /// <summary>
        /// (不要用，檔案過大字串會被截斷) 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <param name="directory"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>每個檔案文字內容</returns>
        Task<string?> GetTodayFileContentAsync(string jobGuid, string directory, string startsWithFileName, IConfiguration _config, CancellationToken cancellationToken);

        /// <summary>
        /// (不要用，檔案過大字串會被截斷) 分段處理 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <param name="directory"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string?> GetFileContentChunkAsync(string jobGuid, string directory, string startsWithFileName, IConfiguration _config, CancellationToken cancellationToken);
        #endregion

        /// <summary>
        /// 分段處理 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <param name="directory"></param>
        /// <param name="startsWithFileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<string>?> GetFileContentChunkListAsync(string jobGuid, string directory, string startsWithFileName, IConfiguration _config, CancellationToken cancellationToken);

        /// <summary>
        /// 拆解文字檔，找出段落
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <param name="logContent">文字內容</param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>寫入完成之序號</returns>
        Task<MailhunterLogParseResponse> MailhunterLogParseLogAsync(string jobGuid, string logContent, IConfiguration _config, CancellationToken cancellationToken);

        /// <summary>
        /// 拆解List文字檔，找出段落
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <param name="logContentList">文字內容</param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>寫入完成之序號</returns>
        Task<MailhunterLogParseResponse> MailhunterLogParseLogListAsync(string jobGuid, List<string> logContentList, IConfiguration _config, CancellationToken cancellationToken);
    }

}
