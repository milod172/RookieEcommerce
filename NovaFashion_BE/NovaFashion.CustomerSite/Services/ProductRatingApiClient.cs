using System.Net;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductRatingDtos;

namespace NovaFashion.CustomerSite.Services
{
    public class ProductRatingApiClient(HttpClient httpClient)
    {
        public async Task<ProductRatingDto?> GetRatingByOrderAsync(Guid id)
        {
            var query = $"api/orders/{id}/rating";

            var response = await httpClient.GetAsync(query);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProductRatingDto>();
        }

        public async Task<ProductRatingDto> CreateRatingAsync(ProductRatingRequest request)
        {
            var query = $"api/product-rating";
            var response = await httpClient.PostAsJsonAsync(query, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProductRatingDto>() ?? new();
        }

    }
}
