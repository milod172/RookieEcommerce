using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.CategoryDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages.Products
{
    public class ProductsModel(ProductApiClient productApi, CategoryApiClient categoryApi) : PageModel
    {
        public PaginationResponseDto<ProductDto> Products { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Newest";

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        private const string DefaultStatus = "Active";

        public async Task<IActionResult> OnGetAsync()
        {

            var productTask = productApi.GetProductsAsync(
                PageNumber,
                PageSize,
                SortBy,
                DefaultStatus,
                MinPrice,
                MaxPrice
            );

            var categoryTask = categoryApi.GetCategoriesAsync();

            await Task.WhenAll(productTask, categoryTask);
            Products = await productTask;
            Categories = await categoryTask;

            // Nếu là HTMX request → trả partial
            if (Request.Headers["HX-Request"] == "true")
            {
                return Partial("_ProductListPartial", this);
            }

            return Page();
        }

    }
}
