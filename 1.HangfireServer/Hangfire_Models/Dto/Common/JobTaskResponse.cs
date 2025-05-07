namespace Hangfire_Models.Dto.Common
{
    /// <summary>
    /// 表示 Hangfire 任務排程的實體模型。
    /// </summary>
    public class JobTask
    {
        /// <summary>
        /// 自動遞增的主鍵識別碼。
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        /// 全域唯一識別碼，用來唯一標識任務。
        /// </summary>
        public string? Uuid { get; set; }

        /// <summary>
        /// 任務名稱。
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 任務的請求 URL。
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// HTTP 方法，例如 GET、POST 等。
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// CRON 表達式，定義任務排程時間。
        /// </summary>
        public string? CronExpression { get; set; }

        /// <summary>
        /// 是否啟用該任務。
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 是否已被邏輯刪除（軟刪除）。
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 任務建立時間。
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 任務最後更新時間。
        /// </summary>
        public DateTime UpdatedTime { get; set; }
    }

}
