using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;

namespace NovaFashion.API.Features.ProductImages
{
    public class ProductImageRepository(AppDbContext context) : IProductImageRepository
    {
        public async Task AddRangeAsync(IEnumerable<ProductImage> entities, CancellationToken ct)
        {
            await context.ProductImages.AddRangeAsync(entities, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task<int> GetMaxSortOrderAsync(Guid productId, CancellationToken ct)
        {
            //Get the maximum SortOrder; if none exists, return -1 so that adding 1 results in 0.
            var maxOrder = await context.ProductImages
                .Where(x => x.ProductId == productId)
                .MaxAsync(x => (int?)x.SortOrder, ct);

            return maxOrder ?? -1;
        }

        public async Task<bool> HasImagesAsync(Guid productId, CancellationToken ct)
        {
            return await context.ProductImages.AnyAsync(x => x.ProductId == productId, ct);
        }

        public async Task DeleteAndReorderAsync(Guid imageId, Guid productId, CancellationToken ct)
        {
            
            var image = await context.ProductImages.FirstOrDefaultAsync(x => x.Id == imageId, ct);

            if (image == null) return;

            context.ProductImages.Remove(image);
            await context.SaveChangesAsync(ct); 

            await ReorderImagesInternal(productId, ct);
        }

        public async Task UpdateOrderAsync(Guid productId, List<Guid> orderedImageIds, CancellationToken ct)
        {
            var images = await context.ProductImages
                .Where(x => x.ProductId == productId)
                .ToListAsync(ct);

            // loop through list Id and update SortOrder and IsPrimary based on position
            for (int i = 0; i < orderedImageIds.Count; i++)
            {
                var img = images.FirstOrDefault(x => x.Id == orderedImageIds[i]);
                if (img != null)
                {
                    img.SortOrder = i;
                    img.IsPrimary = (i == 0); 
                }
            }

            await context.SaveChangesAsync(ct);
        }

        // (Re-index)
        private async Task ReorderImagesInternal(Guid productId, CancellationToken ct)
        {
            var remainingImages = await context.ProductImages
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            for (int i = 0; i < remainingImages.Count; i++)
            {
                remainingImages[i].SortOrder = i;
                remainingImages[i].IsPrimary = (i == 0);
            }

            await context.SaveChangesAsync(ct);
        }
    }
}
