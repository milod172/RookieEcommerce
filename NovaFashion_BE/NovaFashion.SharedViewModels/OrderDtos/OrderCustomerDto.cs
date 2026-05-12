using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class OrderCustomerDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }
        [JsonPropertyName("order_status")]
        public string OrderStatus { get; set; } = string.Empty;
        [JsonPropertyName("items")]
        public List<OrderItemDto> Items { get; set; }
    
    }
}
