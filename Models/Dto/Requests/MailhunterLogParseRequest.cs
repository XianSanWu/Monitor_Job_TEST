namespace Models.Dto.Requests
{
    public class MailhunterLogParseRequest
    {
        public string? LogContent { get; set; }
        public List<string>? LogContentList { get; set; }
    }
}
