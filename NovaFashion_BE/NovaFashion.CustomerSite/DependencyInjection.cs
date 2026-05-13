using Microsoft.AspNetCore.Authentication.Cookies;
using NovaFashion.CustomerSite.Services;
using NovaFashion.CustomerSite.Services.Auth;

namespace NovaFashion.CustomerSite
{
    public static class DependencyInjection
    {
  
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            string baseUrl)
        {
            // Register attach JWT handler
            services.AddHttpContextAccessor();
            services.AddScoped<JwtCookieService>();
            services.AddTransient<AuthenticatedHttpClientHandler>();

            // Auth API client — (anonymous calls)
            services.AddNovaFashionApiClient<AuthApiClient>(baseUrl)
                    .AddNovaFashionApiClient<ProductApiClient>(baseUrl)
                    .AddNovaFashionApiClient<CategoryApiClient>(baseUrl);


            // Auth API client - (need attach JWT)
            services.AddNovaFashionAuthenticatedApiClient<CartApiClient>(baseUrl);
            services.AddNovaFashionAuthenticatedApiClient<OrderApiClient>(baseUrl);
            services.AddNovaFashionAuthenticatedApiClient<VnPayApiClient>(baseUrl);
            services.AddNovaFashionAuthenticatedApiClient<ProductRatingApiClient>(baseUrl);

            return services;
        }

        public static IServiceCollection AddNovaFashionAuthentication(
            this IServiceCollection services,
            int expireTimeSpan
            )
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access-denied";

                    options.Cookie.Name = "nova_cookie";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.IsEssential = true;

                    options.ExpireTimeSpan = TimeSpan.FromMinutes(expireTimeSpan);
                    options.SlidingExpiration = true;
                });

            return services;
        }
    }
}
