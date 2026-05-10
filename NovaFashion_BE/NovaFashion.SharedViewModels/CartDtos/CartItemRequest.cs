using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.CartDtos
{
    public class CartItemRequest
    {
        [JsonPropertyName("product_variant_id")]
        public Guid ProductVariantId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
