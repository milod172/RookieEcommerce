using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Shared.Pagination;

namespace NovaFashion.API.Shared.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationList<T>> PaginateAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            var count = await query.CountAsync(ct);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginationList<T>(items, count, pageNumber, pageSize);
        }
    }
}
