using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels.CartDtos;
using NovaFashion.SharedViewModels.OrderDtos;

namespace NovaFashion.CustomerSite.Pages.Orders
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class OrdersCheckoutModel(CartApiClient cartApi, VnPayApiClient vnPayApi, OrderApiClient orderApi) : PageModel
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

        public async Task<IActionResult> OnPostCheckoutAsync([FromBody] OrderCheckoutRequest request)
        {
            if (request is null || request.Items is not { Count: > 0 })
                return new JsonResult(new { error = "Invalid request" }) { StatusCode = 400 };

            var orderResponse = await orderApi.CreateOrderAsync(request);

            if (!orderResponse.IsSuccessStatusCode)
            {
                var error = await orderResponse.Content.ReadAsStringAsync();
                return new JsonResult(new { error }) { StatusCode = (int)orderResponse.StatusCode };
            }

            var order = await orderResponse.Content.ReadFromJsonAsync<OrderDetailsDto>();

            if (request.PaymentMethod == "vnpay")
            {
                var vnpayResponse = await vnPayApi.CreatePaymentUrlAsync(order!.Id);

                if (!vnpayResponse.IsSuccessStatusCode)
                    return new JsonResult(new { error = "Không tạo được link thanh toán" })
                    {
                        StatusCode = (int)vnpayResponse.StatusCode
                    };

                var paymentUrl = await vnpayResponse.Content.ReadFromJsonAsync<string>();
                return new JsonResult(new { redirect = paymentUrl });
            }

            return new JsonResult(new { redirect = "/orders/success" });
        }
    }
}
