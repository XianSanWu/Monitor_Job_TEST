using Hangfire.Dashboard;
using Hangfire_Utilities.Utilities;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Hangfire.Filters
{
    public class DashboardBasicAuthorizationFilter(IConfiguration config) : IDashboardAuthorizationFilter
    {
        public readonly IConfiguration _config = config;
        private Dictionary<string, string> Users => _config.GetSection("Users").Get<Dictionary<string, string>>() ?? [];
        private string key => _config["EncryptionSettings:AESKey"] ?? string.Empty;
        private string iv => _config["EncryptionSettings:AESIV"] ?? string.Empty;

        private readonly int sessionTimeoutMinutes = 10;

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // 強制 HTTPS
            if (!httpContext.Request.IsHttps)
            {
                RedirectToHttps(httpContext);
                return false;
            }

            // 檢查 Session 是否存在且未過期
            if (IsSessionValid(httpContext))
            {
                return true;
            }

            // 嘗試使用 Basic Auth 登入
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && TryAuthenticateWithBasicAuth(authHeader, httpContext))
            {
                SetSession(httpContext);
                return true;
            }

            // 未登入或過期
            RequestAuthentication(httpContext, "Session expired or not found. Please log in.");
            return false;
        }

        private static void RedirectToHttps(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 301;
            httpContext.Response.Headers.Location = $"https://{httpContext.Request.Host}{httpContext.Request.Path}";
        }

        private bool IsSessionValid(HttpContext httpContext)
        {
            var timestampStr = httpContext.Session.GetString("SessionTimestamp");
            if (string.IsNullOrEmpty(timestampStr)) return false;

            if (DateTime.TryParse(timestampStr, out var timestamp))
            {
                return DateTime.Now.Subtract(timestamp).TotalMinutes <= sessionTimeoutMinutes;
            }

            return false;
        }

        private static void SetSession(HttpContext httpContext)
        {
            httpContext.Session.SetString("SessionTimestamp", DateTime.Now.ToString("O")); // ISO 格式
                                                                                           //httpContext.Session.SetString("IsLoggedIn", "true");
        }

        private bool TryAuthenticateWithBasicAuth(string authHeader, HttpContext httpContext)
        {
            try
            {
                var auth = AuthenticationHeaderValue.Parse(authHeader);
                if (auth.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                {
                    var credentialBytes = Convert.FromBase64String(auth.Parameter ?? string.Empty);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                    Users.TryGetValue(key: (credentials[0] ?? string.Empty), value: out var password);
                    password = CryptoUtil.Decrypt(Base64Util.Decode(password), key, iv);
                    if (credentials.Length == 2 &&
                        //credentials[0].Equals(account, StringComparison.OrdinalIgnoreCase) &&
                        credentials[1].Equals(password, StringComparison.OrdinalIgnoreCase))
                    {
                        var claims = new[]
                        {
                        new Claim(ClaimTypes.Name, credentials[0]),
                        new Claim(ClaimTypes.Role, "admin")
                    };
                        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
                        return true;
                    }
                }
            }
            catch
            {
                // 忽略錯誤
            }

            return false;
        }

        private static void RequestAuthentication(HttpContext httpContext, string message)
        {
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = 401;
                httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
                httpContext.Response.ContentType = "text/plain";
                httpContext.Response.WriteAsync(message).Wait();
            }
        }
    }
}