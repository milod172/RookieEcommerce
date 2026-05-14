using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.CategoryDtos;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductRatingDtos;

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

        public async Task<PaginationResponseDto<ProductRatingDto>> GetRatingByProductAsync(Guid id, int page, int pageSize)
        {
         
            var result = await httpClient.GetFromJsonAsync<PaginationResponseDto<ProductRatingDto>>(
                $"api/products/{id}/rating?PageNumber={page}&PageSize={pageSize}"
            );
            return result ?? throw new Exception($"Lỗi khi query rating bên trong sản phẩm {id}"); ;
            
        }
    }
}
