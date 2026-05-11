using System.Text.Json.Serialization;
using NovaFashion.SharedViewModels.CartDtos;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class OrderDetailsDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        [JsonPropertyName("create_time")]
        public string CreateTime { get; set;} = string.Empty;
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;
        [JsonPropertyName("order_status")]
        public string OrderStatus { get; set; } = string.Empty;
        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }
        [JsonPropertyName("order_items")]
        public List<CartItemDto> OrderItems { get; set; } = [];
    }
}
