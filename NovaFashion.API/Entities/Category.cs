using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class Category : IHasKey<Guid>, IHasAudit
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = [];
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
