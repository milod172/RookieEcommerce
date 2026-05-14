using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductRatingDtos;

namespace NovaFashion.CustomerSite.Pages.Orders
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class OrderDetailsModel(OrderApiClient orderApi, ProductRatingApiClient ratingApi) : PageModel
    {
        public OrderDetailsDto Order { get; set; } = new();
        public ProductRatingDto? Rating { get; set; } = new();

        [BindProperty]
        public ProductRatingRequest Request { get; set; } = new();

        public async Task OnGetAsync(Guid id)
        {
            Order = await orderApi.GetOrderDetailsAsync(id);
            Rating = await ratingApi.GetRatingByOrderAsync(id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Order = await orderApi.GetOrderDetailsAsync(Request.OrderId);
                Rating = await ratingApi.GetRatingByOrderAsync(Request.OrderId);

                return Page();
            }
            var ratingResponse = await ratingApi.CreateRatingAsync(Request);
            return RedirectToPage(new { id = Request.OrderId });
        }
    }
}
