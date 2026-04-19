using NovaFashion.API.Entities;
using NovaFashion.API.Shared.Pagination;

namespace NovaFashion.API.Features.Categories
{
    public interface ICategoryRepository
    {
        Task<PaginationList<Category>> GetPagedCategoriesAsync(PaginationQuery query, CancellationToken ct);
        Task<Category?> FindAsync(Guid id, CancellationToken ct);
        Task AddAsync(Category category, CancellationToken ct);
        Task UpdateAsync(Category category, CancellationToken ct);
        Task DeleteAsync(Category category, CancellationToken ct);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    }
}
