using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Services
{
    public class ProductApiClient(HttpClient httpClient)
    {

        public async Task<HttpResponseMessage> GetProductsAsync(int page, int pageSize, string sort, string filterStatus, decimal? minPrice = null, decimal? maxPrice = null, Guid? categoryId = null)
        {
            var query = $"api/products?PageNumber={page}" +
                $"&PageSize={pageSize}" +
                $"&SortBy={Uri.EscapeDataString(sort)}" +
                $"&Status={filterStatus}";

            if (minPrice.HasValue) query += $"&MinPrice={minPrice.Value}";
            if (maxPrice.HasValue) query += $"&MaxPrice={maxPrice.Value}";
            if (categoryId.HasValue) query += $"&CategoryId={categoryId.Value}";

            var response = await httpClient.GetAsync(query);

            return response;
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(Guid id)
        {
            var query = $"api/products/{id}";

            return await httpClient.GetFromJsonAsync<ProductDetailsDto>(query)
                   ?? new();
        }
    }
}
