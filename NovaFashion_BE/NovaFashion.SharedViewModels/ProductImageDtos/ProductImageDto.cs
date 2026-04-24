namespace NovaFashion.SharedViewModels.ProductImageDtos
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
        public Guid ProductId { get; set; }

    }
}
