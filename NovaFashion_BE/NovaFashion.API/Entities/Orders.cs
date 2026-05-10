using System.Net;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class Orders : IHasAudit
    {
        public Guid Id { get; set; }  = Guid.NewGuid();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionId { get; set; }       //ID from payment gateway
        public DateTime? PaymentDate { get; set; }
        public string? CustomerId { get; set; }
        public virtual ApplicationUser? Customer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
