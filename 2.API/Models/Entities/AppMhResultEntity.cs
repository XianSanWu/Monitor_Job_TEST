namespace Models.Entities
{
    public class AppMhResultEntity
    {
        public int ResultId { get; set; }
        public int ProjectId { get; set; }
        public int? ProjectResultId { get; set; }
        public int? SendUserId { get; set; }
        public int? ResultStatus { get; set; }
        public string? ProtocolDetail { get; set; }
        public string? SendName { get; set; }
        public string? SendEmail { get; set; }
        public DateTime? SendDate { get; set; }
        public int? RetryTimes { get; set; }
        public string? MailExchanger { get; set; }
        public string? MailServerAddress { get; set; }
        public string? SenderIp { get; set; }
        public int? Cfls { get; set; }
        public int? ProjectSecondFeedback { get; set; }
        public string? QueryCode { get; set; }
    }

}
