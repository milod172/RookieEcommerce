using Microsoft.EntityFrameworkCore;

namespace NovaFashion.API.Shared.Pagination
{
    public class PaginationList<T>(List<T> items, int count, int pageNumber, int pageSize)
    {
        public List<T> Items { get; set; } = items;
        public int TotalCount { get; set; } = count;
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginationList<T>> CreateAsync(
            IQueryable<T> query,
            int pageSize,
            int pageNumber,
            CancellationToken ct = default)
        {
            int count = await query.CountAsync(ct);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginationList<T>(items, count, pageNumber, pageSize);
        }
    }
}
