using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;

namespace NovaFashion.API.Features.Categories
{
    public class CategoryRepository(AppDbContext db) : ICategoryRepository
    {
        public async Task<PaginationList<Category>> GetPagedCategoriesAsync(PaginationQuery req, CancellationToken ct)
        {
            var query = db.Categories.AsNoTracking();

            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedTime);
            }

            return await query.PaginateAsync(
                req.PageNumber,
                req.PageSize,
                ct);
        }

        public async Task<Category?> FindAsync(Guid id, CancellationToken ct)
        {
            return await db.Categories.FindAsync([id], ct);
        }

        public async Task AddAsync(Category category, CancellationToken ct)
        {
            await db.Categories.AddAsync(category, ct);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Category category, CancellationToken ct)
        {
            db.Categories.Update(category);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Category category, CancellationToken ct)
        {
            db.Categories.Remove(category);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        {
            return await db.Categories.AnyAsync(x => x.Id == id, ct);
        }
    }
}
