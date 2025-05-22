using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Dto.Mail;
using Services.Interfaces;
using Utilities.LogHelper;

namespace Services.Implementations
{
    /// <summary>
    /// 負責處理發送一般通知信件與 Coline 通知信件的服務
    /// </summary>
    /// <remarks>
    /// 建構子：注入 Email 設定與日誌服務
    /// </remarks>
    public class MailService(IConfiguration config, ILogger<MailService> logger) : IMailService
    {
        private readonly IConfiguration _config = config;
        private EmailSettings Settings => _config.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        private readonly ILogger<MailService> _logger = logger;

        /// <summary>
        /// 同時寄送一般信件與 Coline 通知信件，回傳是否成功
        /// </summary>
        public async Task<bool> SendMailAndColineAsync(string subject, string body, string to = "", string cc = "", bool sendToAdmin = false, string jobGuid = "")
        {
            var logSource = $"【{jobGuid}】" + LogHelper.Build<MailService>();

            var mailSuccess = await SendMailAsync(subject, body, to, cc, sendToAdmin).ConfigureAwait(false);
            if (!mailSuccess)
            {
                _logger.LogError($"{logSource} 信件寄送失敗，主旨：{subject}，收件者：{to}，副本：{cc}");
                return false;
            }

            var colineSuccess = await SendColineAsync(subject, body).ConfigureAwait(false);
            if (!colineSuccess)
            {
                _logger.LogWarning($"{logSource} Coline 通知未成功，主旨：{subject}");
            }

            return true;
        }

        /// <summary>
        /// 寄送一般信件，支援主旨、收件人、副本、是否寄送給管理者，回傳是否成功
        /// </summary>
        private async Task<bool> SendMailAsync(string subject, string body, string to, string cc, bool sendToAdmin)
        {
            try
            {
                var message = CreateMimeMessage(subject, body, to, cc, sendToAdmin, Settings.AdminMail);
                using var client = new SmtpClient();
                await client.ConnectAsync(Settings.MailServer, Settings.MailPort, false).ConfigureAwait(false);
                await client.AuthenticateAsync(Settings.SenderAccount, Settings.SenderPassword).ConfigureAwait(false);
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
                _logger.LogInformation("主信件寄送成功，主旨：{Subject}，收件者：{To}", subject, to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寄送主信件時發生錯誤，主旨：{Subject}，收件者：{To}，副本：{Cc}", subject, to, cc);
                return false;
            }
        }

        /// <summary>
        /// 寄送 Coline 通知信件，根據設定是否啟用，回傳是否成功
        /// </summary>
        private async Task<bool> SendColineAsync(string subject, string body)
        {
            if (Settings.SendToColine?.ToUpper() != "ON")
            {
                _logger.LogInformation("SendToColine 為 OFF，略過 Coline 通知，主旨：{Subject}", subject);
                return false;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Settings.SenderDisplayName, Settings.SenderAccount));
                message.To.Add(MailboxAddress.Parse(Settings.ColineMail));
                message.Subject = $"[{Settings.SenderDisplayName}][{Settings.Environment}] {subject}";
                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(Settings.MailServer, Settings.MailPort, false).ConfigureAwait(false);
                await client.AuthenticateAsync(Settings.SenderAccount, Settings.SenderPassword).ConfigureAwait(false);
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);

                _logger.LogInformation("Coline 通知信寄送成功，主旨：{Subject}", subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寄送 Coline 通知信失敗，主旨：{Subject}", subject);
                return false;
            }
        }

        /// <summary>
        /// 建立 MIME 格式的郵件訊息，包含收件者、副本、管理者等資訊
        /// </summary>
        private MimeMessage CreateMimeMessage(string subject, string body, string to, string cc, bool sendToAdmin, List<string> adminMailList)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Settings.SenderDisplayName, Settings.SenderAccount));

            if (!string.IsNullOrWhiteSpace(Settings.MailReply))
                message.ReplyTo.Add(MailboxAddress.Parse(Settings.MailReply));

            if (!string.IsNullOrWhiteSpace(to))
            {
                foreach (var address in to.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries))
                    message.To.Add(MailboxAddress.Parse(address.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(cc))
            {
                foreach (var address in cc.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries))
                    message.Cc.Add(MailboxAddress.Parse(address.Trim()));
            }

            if (sendToAdmin && adminMailList != null)
            {
                foreach (var address in adminMailList)
                    message.To.Add(MailboxAddress.Parse(address));
            }

            message.Subject = $"[{Settings.Environment}] {subject}";
            var eventTime = $"<br>時間:{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
            message.Body = new TextPart("html") { Text = $"{body}{eventTime}" };
            return message;
        }
    }
}
