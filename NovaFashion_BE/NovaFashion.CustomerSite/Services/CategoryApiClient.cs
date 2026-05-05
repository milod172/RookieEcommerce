using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.CategoryDtos;
using static System.Net.WebRequestMethods;

namespace NovaFashion.CustomerSite.Services
{
    public class CategoryApiClient(HttpClient httpClient)
    {
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var result = await httpClient.GetFromJsonAsync<PaginationResponseDto<CategoryDto>>(
                "/api/categories?PageNumber=1&PageSize=100&Status=Active"
            );
            return result?.Items ?? [];
        }
    }
}
