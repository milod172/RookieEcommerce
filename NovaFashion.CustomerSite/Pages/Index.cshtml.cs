using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public PaginationResponseDto<ProductDto> Products { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("NovaFashion.API");
        }

        public async Task OnGetAsync(int? pageNumber, string? sortBy)
        {
            var query = new StringBuilder("api/products?")
                .Append($"PageNumber={pageNumber ?? 1}&")
                .Append($"PageSize=5&")
                .Append($"SortBy={Uri.EscapeDataString(sortBy ?? "Id desc")}");

            var response = await _httpClient.GetFromJsonAsync<PaginationResponseDto<ProductDto>>(
                query.ToString()
            );

            if (response is not null)
                Products = response;
        }
    }
}
