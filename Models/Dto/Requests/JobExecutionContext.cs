namespace Models.Dto.Requests
{
    public class JobExecutionContext
    {
        public string? JobKey { get; set; } = "";
        public string? JobId { get; set; } = "";
        public string? SelectMethod { get; set; } = "";
        public string? CurrentExecutionId { get; set; } = "";

    }
}
