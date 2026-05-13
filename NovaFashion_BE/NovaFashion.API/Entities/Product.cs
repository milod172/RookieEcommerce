using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class Product :  IHasAudit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
        public decimal? UnitPrice { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int TotalSell { get; set; } = 0;
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public Guid? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = [];
        public virtual ICollection<ProductImage> ProductImages { get; set; } = [];
        public virtual ICollection<ProductRating> ProductRatings { get; set; } = [];

    }
}
