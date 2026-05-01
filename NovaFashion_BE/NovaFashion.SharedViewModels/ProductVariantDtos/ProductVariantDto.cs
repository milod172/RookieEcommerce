
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductVariantDtos
{
    public class ProductVariantDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("product_id")]
        public Guid ProductId { get; set; }
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("size")]  
        public string Size { get; set; } = string.Empty;
        [JsonPropertyName("variant_sku")]   
        public string VariantSku { get; set; } = string.Empty;
        [JsonPropertyName("stock_quantity")]    
        public int StockQuantity { get; set; }
        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }
        [JsonPropertyName("modified_time")]
        public DateTime? ModifiedTime { get; set; }
        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }
    }
}
