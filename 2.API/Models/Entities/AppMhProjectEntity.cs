namespace Models.Entities
{
    public class AppMhProjectEntity
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectCategoryCode { get; set; }
        public int? ProjectStatus { get; set; }
        public int? ProjectGroupId { get; set; }
        public string? ProjectTable { get; set; }
        public int? ProjectOriginTotalUser { get; set; }
        public int? ProjectRemoveUser { get; set; }
        public int? ProjectTotalUser { get; set; }
        public int? ProjectSuccessUser { get; set; }
        public int? OwnerId { get; set; }
        public string? ProjectSenderName { get; set; }
        public string? ProjectSenderEmail { get; set; }
        public string? ProjectReplyName { get; set; }
        public string? ProjectReplyEmail { get; set; }
        public string? ProjectCloseEmail { get; set; }
        public string? ProjectSubject { get; set; }
        public string? ProjectRemark { get; set; }
        public int? ProjectPriority { get; set; }
        public int? ProjectRetryCounter { get; set; }
        public int? ProjectRetryInterval { get; set; }
        public int? ProjectCloseType { get; set; }
        public DateTime? ProjectSendDate { get; set; }
        public int? ProjectSplitTableId { get; set; }
        public int? ProjectFlag { get; set; }
        public int? ProjectIsMultiGroup { get; set; }
        public int? ProjectConfirm { get; set; }
        public int? BlocklistId { get; set; }
        public int? ParentProjectId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? ProjectConfirmThreshold { get; set; }
        public int? ProjectCflsUser { get; set; }
        public int? ProjectEncoderValue { get; set; }
        public int? ProjectFeedback { get; set; }
        public int? ProjectAutoSheduleId { get; set; }
        public int? ProjectSecondId { get; set; }
        public DateTime? ListDownloadDate { get; set; }
        public string? ProjectCompletionEmail { get; set; }
        public int? ListTotal { get; set; }
        public int? BatchNo { get; set; }
        public int? ProjectCompletionEmailFlag { get; set; }
        public string? SendNotice { get; set; }
    }
}
