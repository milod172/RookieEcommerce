using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class OrderItem : IHasAudit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid ProductVariantId { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public Guid OrderId { get; set; }
        public virtual Orders? Order { get; set; }

        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
