using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class GetPaymentSuccessDto
    {
        [JsonPropertyName("order_id")]
        public Guid OrderId { get; set; }

        [JsonPropertyName("create_time")]
        public string CreateTime { get; set; }

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; }

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

    }
}
