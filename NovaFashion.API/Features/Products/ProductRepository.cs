using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;

namespace NovaFashion.API.Features.Products
{
    public class ProductRepository(AppDbContext db) : IProductRepository
    {
        public async Task<PaginationList<Product>> GetPagedProductsAsync(PaginationQuery req, CancellationToken ct)
        {
            var query = db.Products.AsNoTracking();

            //// Filtering 
            //if (!string.IsNullOrWhiteSpace(req.SearchTerm))
            //{
            //    query = query.Where(p => p.ProductName.Contains(req.SearchTerm));
            //}

            //  Sorting 
            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedTime);
            }

            return await query.PaginateAsync(
                req.PageNumber,
                req.PageSize,
                ct);
        }
        public async Task<Product?> FindAsync(Guid id, CancellationToken ct)
        {
            return await db.Products.FindAsync([id], ct);
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            await db.Products.AddAsync(product, ct);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Product product, CancellationToken ct)
        {       
            db.Products.Update(product);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Product product, CancellationToken ct)
        {
            db.Products.Remove(product);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        {
            return await db.Products.AnyAsync(x => x.Id == id, ct);
        }
    }
}
