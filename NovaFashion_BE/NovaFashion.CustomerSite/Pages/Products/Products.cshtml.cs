using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages.Products
{
    public class ProductsModel(ProductApiClient productApi) : PageModel
    {
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Newest";

        private const string DefaultStatus = "Active";

        public PaginationResponseDto<ProductDto> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            Products = await productApi.GetProductsAsync(
                PageNumber,
                PageSize,
                SortBy,
                DefaultStatus
            );
        }

    
    }
}
