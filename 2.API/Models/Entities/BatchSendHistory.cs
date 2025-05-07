
namespace Models.Entities
{
    public class BatchSendHistory
    {
        /// <summary> 記錄的唯一識別碼 </summary>
        public int Id { get; set; }

        /// <summary> 發送的唯一識別碼 </summary>
        public Guid SendUuid { get; set; }

        /// <summary> 發送狀態 </summary>
        public int Status { get; set; }

        /// <summary> 訊息內容 </summary>
        public string? Message { get; set; }

        /// <summary> 建立時間 </summary>
        public DateTime? CreateAt { get; set; }
    }

}
