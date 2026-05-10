using NovaFashion.CustomerSite.Services.Auth;

namespace NovaFashion.CustomerSite
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddNovaFashionApiClient<T>(this IServiceCollection services, string baseUrl)
            where T : class
        {
            services.AddHttpClient<T>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }

        public static IServiceCollection AddNovaFashionAuthenticatedApiClient<TClient>(
        this IServiceCollection services,
        string baseUrl)
        where TClient : class
        {
            services.AddHttpClient<TClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            return services;
        }
    }
}
