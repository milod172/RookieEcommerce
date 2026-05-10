using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class ProductVariant : IHasAudit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Size Size { get; set; }
        public string VariantSku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
