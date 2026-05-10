using NovaFashion.SharedViewModels.CartDtos;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class OrderDetailsDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CreateTime { get; set;} = string.Empty; 
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public List<CartItemDto> OrderItems { get; set; } = [];
    }
}
