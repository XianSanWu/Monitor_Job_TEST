using Hangfire.Dashboard;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Hangfire.Filters
{
    public class DashboardBasicAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string account = "admin_it";
        private readonly string password = "admin_it";
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

            // 檢查 Session 是否過期或不存在
            if (IsSessionExpired(httpContext))
            {
                // 強制重新登入
                RequestAuthentication(httpContext, "Session expired or not found. Please log in.");
            }

            // 驗證帳密 (Basic Auth)
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && TryAuthenticateWithBasicAuth(authHeader, httpContext))
            {
                // Session 存在且未過期，授權通過
                UpdateSessionTimestamp(httpContext);
                return true;
            }

            // 強制重新登入
            RequestAuthentication(httpContext, "Session expired or not found. Please log in.");
            return false;
        }

        private void RedirectToHttps(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 301;
            httpContext.Response.Headers.Location = $"https://{httpContext.Request.Host}{httpContext.Request.Path}";
        }
        private bool IsSessionExpired(HttpContext httpContext)
        {
            var sessionTimestamp = httpContext.Session.GetString("SessionTimestamp");

            if (string.IsNullOrWhiteSpace(sessionTimestamp))
            {
                // No session data found, treat as expired.
                return true;
            }

            if (DateTime.TryParse(sessionTimestamp, out var lastActivityTime))
            {
                return DateTime.Now.Subtract(lastActivityTime).TotalMinutes > sessionTimeoutMinutes;
            }

            return true; // If the session timestamp is not parsable, treat as expired.
        }

        private static void UpdateSessionTimestamp(HttpContext httpContext)
        {
            httpContext.Session.SetString("SessionTimestamp", DateTime.Now.ToString("O")); // Use ISO 8601 format for consistency
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

                    if (credentials.Length == 2 &&
                        credentials[0].Equals(account, StringComparison.OrdinalIgnoreCase) &&
                        credentials[1].Equals(password, StringComparison.OrdinalIgnoreCase))
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, credentials[0]),
                            new Claim(ClaimTypes.Role, "admin")
                        };

                        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
                        UpdateSessionTimestamp(httpContext);
                        return true;
                    }
                }
            }
            catch
            {
                // 忽略解析失敗
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
