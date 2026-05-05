

using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.CategoryDtos
{
    public class CategoryDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("has_children")]
        public bool HasChildren { get; set; }
        public int SubCount { get; set; }
        public Guid? ParentCategoryId { get; set; }
        [JsonPropertyName("sub_categories")]
        public List<CategoryDto> SubCategories { get; set; } = [];
        public bool IsDeleted { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
