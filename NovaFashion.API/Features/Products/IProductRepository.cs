using NovaFashion.API.Entities;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public interface IProductRepository
    {
        Task<PaginationList<Product>> GetPagedProductsAsync(PaginationQuery query, CancellationToken ct);
        Task<Product?> FindAsync(Guid id, CancellationToken ct);
        Task AddAsync(Product product, CancellationToken ct);
        Task UpdateAsync(Product product, CancellationToken ct);
        Task DeleteAsync(Product product, CancellationToken ct);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    }
}
