using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class ProductRating : IHasAudit
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public virtual ApplicationUser Customer { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid OrderId { get; set;}
        public virtual Orders Orders { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }

        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
