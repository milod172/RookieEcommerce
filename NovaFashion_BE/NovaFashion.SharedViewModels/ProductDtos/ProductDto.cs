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

        [JsonPropertyName("unit_price")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("images")]
        public List<ProductImageInProductDto> Images { get; set; } = [];
    }

}

