using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class CartItem : IHasAudit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; private set; }

        public Guid CartId { get; private set; }
        public virtual Cart? Cart { get; private set; }

        public Guid ProductId { get; private set; }
        public virtual Product? Product { get; private set; }

        public Guid? ProductVariantId { get; private set; }
        public virtual ProductVariant? ProductVariant { get; private set; }

        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
