using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.OrderDtos;

namespace NovaFashion.CustomerSite.Pages.Orders
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class OrdersListModel(OrderApiClient orderApi) : PageModel
    {
        public PaginationResponseDto<OrderCustomerDto> Orders { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Newest";

        private const string DefaultStatus = "Active";

        public async Task<IActionResult> OnGetAsync()
        {
            Orders = await orderApi.GetOrdersAsync(
                PageNumber,
                PageSize,
                SortBy,
                DefaultStatus);

            return Page();
        }
    }
}
