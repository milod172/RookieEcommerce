using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class ProductVariant : IHasKey<Guid>, IHasAudit
    {
        public Guid Id { get; set; }
        public Size Size { get; set; }
        public string VariantSku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }

        //public virtual ICollection<CartItem> CartItems { get; set; } = [];
        //public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
