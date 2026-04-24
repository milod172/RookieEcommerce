using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages.Products
{
    public class ProductDetailsModel(ProductApiClient productApi) : PageModel
    {

        public ProductDetailsDto Product { get; set; } = new();
        public async Task OnGetAsync(Guid id)
        {
            Product = await productApi.GetProductByIdAsync(id);
        }
    }
}
