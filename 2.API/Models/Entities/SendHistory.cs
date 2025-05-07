
namespace Models.Entities
{
    public class SendHistory
    {
        /// <summary>記錄的唯一識別碼（自增主鍵）</summary>
        public int Id { get; set; }

        /// <summary>發送的唯一識別碼</summary>
        public string? SendUuid { get; set; }

        /// <summary>發送狀態</summary>
        public string? Status { get; set; }

        /// <summary>訊息內容</summary>
        public string? Message { get; set; }

        /// <summary>記錄建立時間</summary>
        public DateTime? CreateAt { get; set; }
    }

}
