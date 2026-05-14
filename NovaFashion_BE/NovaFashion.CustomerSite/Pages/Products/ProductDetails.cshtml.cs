using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductRatingDtos;

namespace NovaFashion.CustomerSite.Pages.Products
{
    public class ProductDetailsModel(ProductApiClient productApi) : PageModel
    {
        public PaginationResponseDto<ProductRatingDto> Rating { get; set; } = new();
        public ProductDetailsDto Product { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;
        public async Task OnGetAsync(Guid id)
        {
            Product = await productApi.GetProductByIdAsync(id);
            Rating = await productApi.GetRatingByProductAsync(id, PageNumber, PageSize);
        }
    }
}
