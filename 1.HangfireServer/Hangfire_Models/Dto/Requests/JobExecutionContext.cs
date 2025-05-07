namespace Hangfire_Models.Dto.Requests
{
    public class JobExecutionContext
    {
        public string? JobKey { get; set; } = "";
        public string? JobId { get; set; } = "";
        public string? SelectMethod { get; set; } = "";
        public string? CurrentExecutionId { get; set; } = "";

        public override string ToString()
        {
            return $"排程名稱：{JobKey}。執行：{SelectMethod}（排程建立 ID：{JobId}）";//，CurrentExecutionId：{CurrentExecutionId}
        }
    }

}
