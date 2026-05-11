using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages
{
    public class PaymentSuccessModel(OrderApiClient orderApi) : PageModel
    {
        public GetPaymentSuccessDto SuccessState { get; set; } = new();
        public async Task OnGetAsync(Guid orderId)
        {
            SuccessState = await orderApi.GetPaymentSuccessAsync(orderId);
        }
    }
}
