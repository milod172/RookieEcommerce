using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class Cart : IHasAudit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? CustomerId { get; set; }
        public virtual ApplicationUser? Customer { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; } = [];
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

    }
}
