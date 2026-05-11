using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Pages.Orders
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class OrderDetailsModel(OrderApiClient orderApi) : PageModel
    {
        public OrderDetailsDto Order { get; set; } = new();
        public async Task OnGetAsync(Guid id)
        {
            Order = await orderApi.GetOrderDetailsAsync(id);
        }
    }
}
