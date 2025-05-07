
namespace Models.Entities
{
    public class SmsSendHistory
    {
        /// <summary>簡訊流水號</summary>
        public string? RowId { get; set; }

        /// <summary>客戶編號</summary>
        public string? CiInfo { get; set; }

        /// <summary>訊息狀態: None(初始), Sent(發送至SMS API), Failed(發送至SMS API失敗/查詢SMS狀態回傳失敗), Pending(SMS API處理中), Delivered(已送達至客戶)</summary>
        public string? SmsSendStatus { get; set; }

        /// <summary>SMS API回傳訊息</summary>
        public string? Message { get; set; }

        /// <summary>記錄建立時間</summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>記錄最後更新時間</summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>批次號</summary>
        public string? BatchId { get; set; }

        /// <summary>簡訊內容</summary>
        public string? Content { get; set; }

        /// <summary>傳送回饋API次數(Retry用)</summary>
        public int? SentToFeedbackApiTimes { get; set; }

        /// <summary>愛酷事件ID</summary>
        public string? AccuUuid { get; set; }

        /// <summary>執行階段: SentToSmsApiSuccess, SentToSmsApiFailed, SentToFeedbackApiSuccess, SentToFeedbackApiFailed</summary>
        public string? ExecutedStage { get; set; }
    }

}
