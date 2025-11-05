namespace Hangfire_Services.Interfaces
{
    public interface IApiService
    {
        Task<string?> CallApiAsync(string apiUrl, HttpMethod method, Dictionary<string, string>? queryParams = null, string? bodyContent = null);
    }
}
