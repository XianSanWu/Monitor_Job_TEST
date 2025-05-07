
namespace Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendMailAndColineAsync(string subject, string body, string to = "", string cc = "", bool sendToAdmin = false, string jobGuid = "");
    }
}
