namespace NovaFashion.SharedViewModels.ProductDtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
    }
}
