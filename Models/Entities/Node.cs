
namespace Models.Entities
{
    public class Node
    {
        /// <summary>記錄的唯一識別碼（自增主鍵）</summary>
        public int Id { get; set; }

        /// <summary>節點的唯一識別碼</summary>
        public string? NodeId { get; set; }

        /// <summary>節點名稱</summary>
        public string? NodeName { get; set; }

        /// <summary>所屬旅程的唯一識別碼</summary>
        public string? ActivityId { get; set; }

        /// <summary>通道類型</summary>
        public string? Channel { get; set; }

        /// <summary>記錄建立時間</summary>
        public DateTime? CreateAt { get; set; }

        /// <summary>記錄最後更新時間</summary>
        public DateTime? UpdateAt { get; set; }
    }

}
