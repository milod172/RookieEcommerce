using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.CartDtos
{
    public class CartItemDto
    {
        [JsonPropertyName("product_variant_id")]
        public Guid ProductVariantId { get; set; }
        [JsonPropertyName("product_id")]
        public Guid ProductId { get; set; }

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;
        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;
        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("total_price")]
        public decimal TotalPrice { get; set; }
        [JsonPropertyName("available_stock")]
        public int AvailableStock { get; set; }
        [JsonPropertyName("is_available")]
        public bool IsAvailable { get; set; }
        [JsonPropertyName("is_exceed_stock")]
        public bool IsExceedStock { get; set; }
    }
}
