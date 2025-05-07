
namespace Models.Entities
{
    public class Journey
    {
        /// <summary> 記錄的唯一識別碼（自增主鍵） </summary>
        public int Id { get; set; }

        /// <summary> 旅程的唯一識別碼 </summary>
        public string? ActivityId { get; set; }

        /// <summary> 旅程名稱 </summary>
        public string? ActivityName { get; set; }

        /// <summary> 旅程狀態 </summary>
        public string? Status { get; set; }

        /// <summary> 旅程開始時間 </summary>
        public DateTime? StartAt { get; set; }

        /// <summary> 旅程結束時間 </summary>
        public DateTime? StopAt { get; set; }

        /// <summary> 記錄建立時間 </summary>
        public DateTime? CreateAt { get; set; }

        /// <summary> 記錄最後更新時間 </summary>
        public DateTime? UpdateAt { get; set; }
    }

}
