using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.CategoryDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages.Products
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
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

        [BindProperty(SupportsGet = true)]
        public Guid? CategoryId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ModelState.Clear();
            var response = await productApi.GetProductsAsync(
                PageNumber,
                PageSize,
                SortBy,
                DefaultStatus,
                MinPrice,
                MaxPrice,
                CategoryId
            );

            if (Request.Headers.ContainsKey("HX-Request"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

                    if (apiError is not null)
                    {
                        ModelState.Clear();
                        foreach (var (key, messages) in apiError.Errors)
                            foreach (var msg in messages)
                                ModelState.AddModelError(key, msg);
                    }

                    return Partial("_ProductContainerPartial", this);
                }

               
                Products = await response.Content.ReadFromJsonAsync<PaginationResponseDto<ProductDto>>() ?? new();
                return Partial("_ProductContainerPartial", this);
            }

            Products = await response.Content.ReadFromJsonAsync<PaginationResponseDto<ProductDto>>() ?? new();
            return Page();
        }

        
        //public async Task<IActionResult> OnGetCategoryFilterPartialAsync([FromQuery] Guid? categoryId)
        //{
        //    CategoryId = categoryId;
        //    ViewData["CategoryId"] = CategoryId;

        //    Categories = await categoryApi.GetCategoriesAsync();

        //    return Partial("_CategoryFilterPartial", Categories);
        //}

        //public async Task<IActionResult> OnGetAsync()
        //{
        //    ViewData["CategoryId"] = CategoryId;

        //    var response = await productApi.GetProductsAsync(
        //        PageNumber,
        //        PageSize,
        //        SortBy,
        //        DefaultStatus,
        //        MinPrice,
        //        MaxPrice,
        //        CategoryId
        //    );

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //        {
        //            var rawContent = await response.Content.ReadAsStringAsync();
        //            var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

        //            if (apiError is not null)
        //            {

        //                foreach (var (key, messages) in apiError.Errors)
        //                    foreach (var msg in messages)
        //                    {

        //                        ModelState.AddModelError(key, msg);
        //                    }           
        //            }
        //        }

        //        if (Request.Headers["HX-Request"] == "true")
        //        {
        //            Console.WriteLine("Trả _ProductFilterPartial");
        //            Response.Headers["HX-Retarget"] = "#product-filter-sidebar";
        //            Response.Headers["HX-Reswap"] = "innerHTML";
        //            return Partial("_ProductFilterPartial", this);
        //        }

        //        return Page();
        //    }

        //    ModelState.Clear();
        //    Products = await response.Content.ReadFromJsonAsync<PaginationResponseDto<ProductDto>>() ?? new();

        //    var categoryTask = categoryApi.GetCategoriesAsync();
        //    Categories = await categoryTask;

        //    // Nếu là HTMX request → trả partial
        //    if (Request.Headers["HX-Request"] == "true")
        //    {
        //    Response.Headers["HX-Retarget"] = "#product-container";
        //    Response.Headers["HX-Reswap"] = "innerHTML"; 
        //    Response.Headers["HX-Trigger"] = "filterSuccess";
        //    return Partial("_ProductListPartial", this);
        //    }

        //    return Page();

        //}
    }
}
