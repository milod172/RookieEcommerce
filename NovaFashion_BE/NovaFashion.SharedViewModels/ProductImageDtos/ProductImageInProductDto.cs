using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductImageDtos
{
    public class ProductImageInProductDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("alt_text")]
        public string AltText { get; set; } = string.Empty;

        [JsonPropertyName("sort_order")]
        public int SortOrder { get; set; }

        [JsonPropertyName("is_primary")]
        public bool IsPrimary { get; set; }
    }
}
