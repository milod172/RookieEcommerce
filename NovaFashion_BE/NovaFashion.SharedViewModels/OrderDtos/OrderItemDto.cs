using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class OrderItemDto
    {
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;
        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty ;
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("total_price")]
        public decimal TotalPrice { get; set; }
    }
}
