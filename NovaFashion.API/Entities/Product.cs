using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Entities
{
    public class Product : IHasKey<Guid>, IHasAudit
    {
        public Guid Id { get; set; }
        public string ProductName { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
