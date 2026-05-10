using NovaFashion.SharedViewModels.CartDtos;

namespace NovaFashion.CustomerSite.Services
{
    public class CartApiClient(HttpClient httpClient)
    {
        public async Task<HttpResponseMessage> GetProductsInCartAsync(List<CartItemRequest> items)
        {
            return await httpClient.PostAsJsonAsync("api/carts", items);
        }
    }
}
