namespace NovaFashion.CustomerSite.Services.Auth
{
    /// <summary>
    /// DelegatingHandler tự động attach JWT Bearer token vào mọi HttpClient request
    /// từ Razor Pages server → Backend API
    /// </summary>
    public class AuthenticatedHttpClientHandler(
        IHttpContextAccessor httpContextAccessor,
        JwtCookieService jwtCookieService,
        ILogger<AuthenticatedHttpClientHandler> logger) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = httpContextAccessor.HttpContext;

            if (context != null)
            {
                var token = jwtCookieService.GetAccessToken(context);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    logger.LogDebug("No access token found for request to {Uri}", request.RequestUri);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
