using System.Text.Json.Serialization;
using NovaFashion.SharedViewModels.ProductImageDtos;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.SharedViewModels.ProductDtos
{
    public class ProductDetailsDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("unit_price")]
        public decimal? UnitPrice { get; set; }
        [JsonPropertyName("details")]
        public string? Details { get; set; } = string.Empty;
        [JsonPropertyName("total_quantity")]    
        public int TotalQuantity { get; set; } = 0;
        [JsonPropertyName("total_sell")]
        public int TotalSell { get; set; } = 0;
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;
        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }
        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = string.Empty;
        [JsonPropertyName("images")]
        public List<ProductImageInProductDto> Images { get; set; } = [];

        [JsonPropertyName("variants")]
        public List<ProductVariantInProductDto> Variants { get; set; } = [];

        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }
        public string? CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
