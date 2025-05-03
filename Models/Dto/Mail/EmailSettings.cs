namespace Models.Dto.Mail
{
    /// <summary>電子郵件相關設定</summary>
    public class EmailSettings
    {
        /// <summary>SMTP 伺服器位址</summary>
        public string MailServer { get; set; } = string.Empty;

        /// <summary>SMTP 伺服器連接埠，預設為 25</summary>
        public int MailPort { get; set; } = 25;

        /// <summary>發信帳號</summary>
        public string SenderAccount { get; set; } = string.Empty;

        /// <summary>發信帳號密碼（可加密）</summary>
        public string SenderPassword { get; set; } = string.Empty;

        /// <summary>發信人顯示名稱</summary>
        public string SenderDisplayName { get; set; } = string.Empty;

        /// <summary>回覆信件的 Email 位址</summary>
        public string MailReply { get; set; } = string.Empty;

        /// <summary>管理員收件者清單</summary>
        public List<string> AdminMail { get; set; } = new();

        /// <summary>使用者收件者清單</summary>
        public List<string> UserMail { get; set; } = new();

        /// <summary>目前執行環境（例如 Development、UAT、Production）</summary>
        public string Environment { get; set; } = "Development";

        /// <summary>是否開啟傳送至 Coline 的功能（ON / OFF）</summary>
        public string SendToColine { get; set; } = "ON";

        /// <summary>Coline 通知收件者 Email</summary>
        public string ColineMail { get; set; } = string.Empty;
    }

}
