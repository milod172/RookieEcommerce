

namespace NovaFashion.SharedViewModels.CategoryDtos
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool HasChildren { get; set; }
        public int SubCount { get; set; }
        public Guid? ParentCategoryId { get; set; }
        //public List<CategoryDto> SubCategories { get; set; } = new();
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
