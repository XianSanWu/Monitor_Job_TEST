namespace Models.Entities
{
    public class WorkflowEntity
    {
        /// <summary>主鍵</summary>
        public int SN { get; set; }

        /// <summary>工作流程唯一識別碼</summary>
        public string? WorkflowUuid { get; set; }

        /// <summary>發送 UUID</summary>
        public string? SendUuid { get; set; }

        /// <summary>SendUuid 排序</summary>
        public int? SendUuidSort { get; set; }

        /// <summary>批次 ID</summary>
        public string? BatchId { get; set; }

        /// <summary>BatchId 排序</summary>
        public int? BatchIdSort { get; set; }

        /// <summary>CDP 批次 ID</summary>
        public string? CdpUuid { get; set; }

        /// <summary>旅程 ID</summary>
        public string? ActivityId { get; set; }

        /// <summary>旅程名稱</summary>
        public string? ActivityName { get; set; }

        /// <summary>旅程狀態</summary>
        public string? ActivityStatus { get; set; }

        /// <summary>節點 ID</summary>
        public string? NodeId { get; set; }

        /// <summary>節點名稱</summary>
        public string? NodeName { get; set; }

        /// <summary>通道 (Email / SMS)</summary>
        public string? Channel { get; set; }

        /// <summary>通道類型 (EDM / 簡訊)</summary>
        public string? ChannelType { get; set; }

        /// <summary>上傳檔名</summary>
        public string? UploadFileName { get; set; }

        /// <summary>進度狀態</summary>
        public string? ProgressStatus { get; set; }

        /// <summary>建立時間</summary>
        public DateTime? CreateAt { get; set; }

        /// <summary>更新時間</summary>
        public DateTime? UpdateAt { get; set; }

        /// <summary>旅程建立時間</summary>
        public DateTime? JourneyCreateAt { get; set; }

        /// <summary>旅程更新時間</summary>
        public DateTime? JourneyUpdateAt { get; set; }

        /// <summary>旅程完成時間</summary>
        public DateTime? JourneyFinishAt { get; set; }

        /// <summary>群發建立時間</summary>
        public DateTime? GroupSendCreateAt { get; set; }

        /// <summary>群發更新時間</summary>
        public DateTime? GroupSendUpdateAt { get; set; }

        /// <summary>愛酷CDP計算總數</summary>
        public int? AccuCdpTotalCount { get; set; }

        /// <summary>Mailhunter成功數</summary>
        public int? MailhunterSuccessCount { get; set; }

        /// <summary>訊息</summary>
        public string? Message { get; set; }
    }
}
