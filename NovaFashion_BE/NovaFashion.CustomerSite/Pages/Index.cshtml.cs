using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.CategoryDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages
{
    public class IndexModel(ProductApiClient productApi, CategoryApiClient categoryApi) : PageModel
    {
        public PaginationResponseDto<ProductDto> Products { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 8;

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Newest";

        [BindProperty(SupportsGet = true)]
        public Guid? CategoryId { get; set; }

        private const string DefaultStatus = "Active";

        public async Task<IActionResult> OnGetAsync()
        {
            var categories = await categoryApi.GetCategoriesAsync();
            categories = categories.Where(c => c.ParentCategoryId == null).ToList();

            Categories = categories;    

            var response = await productApi.GetProductsAsync(
                PageNumber,
                PageSize,
                SortBy,
                DefaultStatus,
                null,
                null,
                CategoryId
            );

            if (response.IsSuccessStatusCode)
            {
                Products = await response.Content
                    .ReadFromJsonAsync<PaginationResponseDto<ProductDto>>() ?? new();
            }

            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return Partial("Products/_ProductListMainPartial", Products);
            }

            return Page();
        }
    }
}
