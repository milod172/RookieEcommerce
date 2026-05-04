using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages
{
    public class IndexModel(ProductApiClient productApi) : PageModel
    {
        public PaginationResponseDto<ProductDto> Products { get; set; } = new();

        public async Task OnGetAsync(int? pageNumber, int? pageSize, string? sortBy)
        {
            Products = await productApi.GetProductsAsync(
                pageNumber ?? 1,
                pageSize ?? 10,
                sortBy ?? "IdDesc",
                "Active"
            );
        }
    }
}
