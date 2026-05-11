using System.Collections;
using NovaFashion.SharedViewModels;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.CustomerSite.Services
{
    public class OrderApiClient(HttpClient httpClient)
    {
        public async Task<PaginationResponseDto<OrderDto>> GetOrdersAsync(
           int page,
           int pageSize,
           string sort,
           string filterStatus)
        {
             var query = $"api/orders/my-orders?PageNumber={page}" +
                $"&PageSize={pageSize}" +
                $"&SortBy={Uri.EscapeDataString(sort)}" +
                $"&Status={filterStatus}";

            return await httpClient.GetFromJsonAsync<PaginationResponseDto<OrderDto>>(query)
                   ?? new PaginationResponseDto<OrderDto>();
        }

        public async Task<HttpResponseMessage> CreateOrderAsync(OrderCheckoutRequest request)
        {
            return await httpClient.PostAsJsonAsync("api/orders/checkout", request);
        }

        public async Task<GetPaymentSuccessDto> GetPaymentSuccessAsync(Guid orderId)
        {
            var query = $"api/orders/payment-success/{orderId}";

            return await httpClient.GetFromJsonAsync<GetPaymentSuccessDto>(query)
                   ?? new();
        }

        public async Task<OrderDetailsDto> GetOrderDetailsAsync(Guid id)
        {
            var query = $"api/orders/{id}";

            return await httpClient.GetFromJsonAsync<OrderDetailsDto>(query)
                   ?? new();
        }
    }
}
