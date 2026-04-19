using NovaFashion.API.Entities;

namespace NovaFashion.API.Features.ProductImages
{
    public interface IProductImageRepository
    {
        Task AddRangeAsync(IEnumerable<ProductImage> entities, CancellationToken ct);
        Task DeleteAndReorderAsync(Guid imageId, Guid productId, CancellationToken ct);
        Task UpdateOrderAsync(Guid productId, List<Guid> orderedImageIds, CancellationToken ct);
        Task<int> GetMaxSortOrderAsync(Guid productId, CancellationToken ct);
        Task<bool> HasImagesAsync(Guid productId, CancellationToken ct);
    }
}
