using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class ProductImage : IHasKey<Guid>
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
