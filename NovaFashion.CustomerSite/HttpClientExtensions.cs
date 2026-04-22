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
    }
}
