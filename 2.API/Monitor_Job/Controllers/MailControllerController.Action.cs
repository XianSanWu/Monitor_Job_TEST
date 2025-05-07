using Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public partial class MailControllerController : BaseController
    {
        /// <summary>
        /// 發送一般信件與 Coline 通知
        /// </summary>
        /// <param name="subject">信件主旨</param>
        /// <param name="body">信件內容 (HTML 格式)</param>
        /// <param name="to">主要收件人 (可多筆, 用逗號分隔)</param>
        /// <param name="cc">副本收件人 (可選, 用逗號分隔)</param>
        /// <param name="sendToAdmin">是否同時寄給管理員</param>
        [Tags("Mail.Action")]  //分組(可多標籤)        
        [HttpPost("send_Mail")]
        public async Task<ResultResponse<bool>> SendAsync(
            [FromQuery] string subject,
            [FromQuery] string body,
            [FromQuery] string to,
            [FromQuery] string? cc = "",
            [FromQuery] bool sendToAdmin = false)
        {
            var result = false;

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body) || string.IsNullOrWhiteSpace(to))
                return SuccessResult(result);

            result = await _mailService.SendMailAndColineAsync(subject, body, to, cc ?? string.Empty, sendToAdmin);
            return SuccessResult(result);
        }
    }
}
