using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingRequest
    {
        [JsonPropertyName("comment")]
        public string Comment { get; set; } = string.Empty;
        [JsonPropertyName("rate")]
        public int Rate { get; set; }
        [JsonPropertyName("order_id")]
        public Guid OrderId { get; set; }
    }
}
