using Hangfire.Dashboard;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Hangfire.Filters
{
    public class DashboardBasicAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string account = "admin_it";
        private readonly string password = "admin_it";
        private readonly int sessionTimeoutMinutes = 10;

        public DashboardBasicAuthorizationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Force HTTPS redirect
            if (!httpContext.Request.IsHttps)
            {
                RedirectToHttps(httpContext);
                return false;
            }

            // Check session expiration
            if (IsSessionExpired(httpContext))
            {
                ForceLogout(httpContext);
                return false;
            }

            // If user is authenticated, authorize
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                // Update session timestamp
                UpdateSessionTimestamp(httpContext);
                return true;
            }

            // Basic Authentication via header
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && TryAuthenticateWithBasicAuth(authHeader, httpContext))
            {
                return true;
            }

            // If not authenticated, force login
            RequestAuthentication(httpContext);
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
            if (!string.IsNullOrWhiteSpace(sessionTimestamp))
            {
                var lastActivityTime = DateTime.Parse(sessionTimestamp);
                return DateTime.Now.Subtract(lastActivityTime).TotalMinutes > sessionTimeoutMinutes;
            }
            return false;
        }

        private static void ForceLogout(HttpContext httpContext)
        {
            httpContext.Session.Remove("SessionTimestamp");
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.WriteAsync("Session expired. Please log in again.").Wait();
        }

        private static void UpdateSessionTimestamp(HttpContext httpContext)
        {
            httpContext.Session.SetString("SessionTimestamp", DateTime.Now.ToString());
        }

        private bool TryAuthenticateWithBasicAuth(string authHeader, HttpContext httpContext)
        {
            var auth = AuthenticationHeaderValue.Parse(authHeader);

            if (auth.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                var credentials = Encoding.UTF8
                    .GetString(Convert.FromBase64String(auth.Parameter ?? string.Empty))
                    .Split(':', 2);

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
            return false;
        }

        private static void RequestAuthentication(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.WriteAsync("Authentication is required.").Wait();
        }
    }
}
