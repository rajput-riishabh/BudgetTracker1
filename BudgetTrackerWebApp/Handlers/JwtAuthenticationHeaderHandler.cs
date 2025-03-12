using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BudgetTrackerWebApp.Handlers
{
    public class JwtAuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthenticationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("JwtAuthenticationHeaderHandler: SendAsync executed.");
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null && httpContext.User.Identity.IsAuthenticated)
            {
                Debug.WriteLine("JwtAuthenticationHeaderHandler: User is authenticated.");
                var authenticationResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (authenticationResult.Succeeded)
                {
                    Debug.WriteLine("JwtAuthenticationHeaderHandler: Authentication succeeded.");

                    // **Retrieve token from cookies - THIS IS THE CORRECT WAY NOW**
                    var token = httpContext.Request.Cookies["authToken"];

                    if (!string.IsNullOrEmpty(token))
                    {
                        Debug.WriteLine($"JwtAuthenticationHeaderHandler: Token retrieved from cookie: {token.Substring(0, 20)}... (truncated)");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        Debug.WriteLine("JwtAuthenticationHeaderHandler: Authorization header added.");
                    }
                    else
                    {
                        Debug.WriteLine("JwtAuthenticationHeaderHandler: Token is NULL or empty from cookie."); // Log if token is missing from cookie
                    }
                }
                else
                {
                    Debug.WriteLine("JwtAuthenticationHeaderHandler: Authentication NOT succeeded.");
                }
            }
            else
            {
                Debug.WriteLine("JwtAuthenticationHeaderHandler: HttpContext is NULL or User NOT authenticated.");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}