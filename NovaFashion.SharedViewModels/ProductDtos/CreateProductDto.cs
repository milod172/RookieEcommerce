

namespace NovaFashion.SharedViewModels.ProductDtos
{
    public class CreateProductDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
        public string Sku { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
    }
}
