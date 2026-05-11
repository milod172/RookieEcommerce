namespace NovaFashion.CustomerSite.Services
{
    public class VnPayApiClient(HttpClient httpClient)
    {
        public async Task<HttpResponseMessage> CreatePaymentUrlAsync(Guid orderId)
        {
            return await httpClient.PostAsJsonAsync("api/vnpay/url-payment", new { order_id = orderId });
        }

    }
}
