using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Services
{
    public class ProductApiClient(HttpClient httpClient)
    {

        public async Task<PaginationResponseDto<ProductDto>> GetProductsAsync(int page, int pageSize, string sort, string filterStatus, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = $"api/products?PageNumber={page}" +
                $"&PageSize={pageSize}" +
                $"&SortBy={Uri.EscapeDataString(sort)}" +
                $"&Status={filterStatus}";

            if (minPrice.HasValue) query += $"&MinPrice={minPrice.Value}";
            if (maxPrice.HasValue) query += $"&MaxPrice={maxPrice.Value}";

            return await httpClient.GetFromJsonAsync<PaginationResponseDto<ProductDto>>(query)
                   ?? new();
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(Guid id)
        {
            var query = $"api/products/{id}";

            return await httpClient.GetFromJsonAsync<ProductDetailsDto>(query)
                   ?? new();
        }
    }
}
