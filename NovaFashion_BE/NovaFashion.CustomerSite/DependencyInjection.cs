using NovaFashion.CustomerSite.Services;

namespace NovaFashion.CustomerSite
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services,string baseUrl)
        {
            services
                .AddNovaFashionApiClient<ProductApiClient>(baseUrl)
                .AddNovaFashionApiClient<CategoryApiClient>(baseUrl);

            return services;
        }
    }
}
