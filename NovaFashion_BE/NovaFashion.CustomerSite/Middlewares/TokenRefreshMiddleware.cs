using NovaFashion.CustomerSite.Services.Auth;

namespace NovaFashion.CustomerSite.Middlewares
{
    public class TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // refresh token for logged user
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var jwtService = context.RequestServices.GetRequiredService<JwtCookieService>();

                if (jwtService.IsAccessTokenExpired(context))
                {
                    var refreshed = await TryRefreshTokenAsync(context, jwtService);

                    if (!refreshed)
                    {
                        //Refresh fail, redirect login
                        logger.LogWarning("Token refresh failed, signing out user");
                        await jwtService.SignOutAsync(context);
                        context.Response.Redirect("/auth/login?expired=true");
                        return;
                    }
                }
            }

            await next(context);
        }

        private static async Task<bool> TryRefreshTokenAsync(
            HttpContext context,
            JwtCookieService jwtService)
        {
            var (userId, refreshToken) = jwtService.GetRefreshInfo(context);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(refreshToken))
                return false;

            var authApiClient = context.RequestServices.GetRequiredService<AuthApiClient>();
            var newToken = await authApiClient.RefreshTokenAsync(userId, refreshToken);

            if (newToken is null) return false;

            await jwtService.SignInAsync(context, newToken);
            return true;
        }
    }
}
