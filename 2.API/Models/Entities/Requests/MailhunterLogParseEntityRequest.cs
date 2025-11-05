namespace Models.Entities.Requests
{
    public class MailhunterLogParseEntityRequest
    {
        public string? LogContent { get; set; }
        public List<string>? LogContentList { get; set; }
    }
}
