using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductVariantDtos
{
    public class ProductVariantInProductDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;
        [JsonPropertyName("variant_sku")]
        public string VariantSku { get; set; } = string.Empty;
        [JsonPropertyName("stock_quantity")]
        public int StockQuantity { get; set; }
        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }
        [JsonPropertyName("is_available")]
        public bool IsAvailable { get; set; }
        [JsonPropertyName("is_exceed_stock")]
        public bool IsExceedStock { get; set; }
    }
}
