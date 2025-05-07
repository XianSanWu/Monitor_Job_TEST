
namespace Hangfire_Servies.Interfaces
{
    public interface IApiService
    {
        Task<string?> CallApiAsync(string apiUrl, HttpMethod method, Dictionary<string, string>? queryParams = null, string? bodyContent = null);
    }
}
