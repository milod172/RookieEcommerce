namespace NovaFashion.CustomerSite.Middlewares
{
    public class StatusCodeMiddleware(RequestDelegate next, ILogger<StatusCodeMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            await next(context);

            // Chỉ xử lý khi chưa có response body (tránh double-write)
            if (context.Response.HasStarted) return;

            // Bỏ qua static files và API calls nếu có
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)) return;

            switch (context.Response.StatusCode)
            {
                case 401:
                    logger.LogWarning("401 Unauthorized: {Path}", path);
                    if (!IsHtmxRequest(context))
                        context.Response.Redirect($"/login?returnUrl={Uri.EscapeDataString(path)}");
                    break;

                case 403:
                    logger.LogWarning("403 Forbidden: {Path} | User: {User}",
                        path, context.User.Identity?.Name);
                    context.Response.Redirect("/access-denied");
                    break;

                case 404:
                    logger.LogInformation("404 Not Found: {Path}", path);
                    context.Response.Redirect("/not-found");
                    break;

                case >= 500:
                    logger.LogError("5xx Server Error {StatusCode}: {Path}",
                        context.Response.StatusCode, path);
                    context.Response.Redirect("/errors/server-error");
                    break;
            }
        }

        private static bool IsHtmxRequest(HttpContext context)
            => context.Request.Headers.ContainsKey("HX-Request");
    }
}
