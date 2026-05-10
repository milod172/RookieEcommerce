using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels.CartDtos;

namespace NovaFashion.CustomerSite.Pages.Orders
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class OrdersCheckoutModel(CartApiClient cartApi) : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLoadOrderAsync([FromBody] List<CartItemRequest> items)
        {
            if (items is not { Count: > 0 })
                return new JsonResult(new List<CartItemDto>());

            var response = await cartApi.GetProductsInCartAsync(items);

            if (!response.IsSuccessStatusCode)
            {
                return new JsonResult(new { error = "Failed to fetch cart" })
                {
                    StatusCode = (int)response.StatusCode
                };
            }

            var result = await response.Content.ReadFromJsonAsync<List<CartItemDto>>() ?? [];

            return new JsonResult(result);
        }
    }
}
