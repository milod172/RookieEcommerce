
namespace NovaFashion.SharedViewModels.ProductVariantDtos
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string VariantSku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
