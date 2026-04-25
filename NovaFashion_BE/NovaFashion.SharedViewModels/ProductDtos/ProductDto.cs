using System.Text.Json.Serialization;
using NovaFashion.SharedViewModels.ProductImageDtos;

namespace NovaFashion.SharedViewModels.ProductDtos
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("sku")]
        public string Sku { get; set;} = string.Empty; 

        [JsonPropertyName("unit_price")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("total_quantity")]
        public int TotalQuantity { get; set; }

        [JsonPropertyName("total_sell")]
        public int TotalSell { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("images")]
        public List<ProductImageInProductDto> Images { get; set; } = [];
    }

}

