using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Shared.Pagination;
using static FastEndpoints.Ep;

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

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy)
        { 
            var sort = sortBy.Trim().ToLower();

            return sort switch
            {
                "id desc" => query.OrderByDescending(x => EF.Property<object>(x, "Id")),
                "id asc" => query.OrderBy(x => EF.Property<object>(x, "Id")),
                _ => query.OrderByDescending(x => EF.Property<object>(x, "Id")) 
            };
        }
    }
}
