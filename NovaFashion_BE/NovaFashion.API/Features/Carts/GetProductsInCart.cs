using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CartDtos;

namespace NovaFashion.API.Features.Carts
{
   
    public class GetProductInCartMapper : Mapper<List<CartItemRequest>, List<CartItemDto>, List<(ProductVariant Variant, int Quantity)>> {
        public override List<CartItemDto> FromEntity(List<(ProductVariant Variant, int Quantity)> e)
        {
            return e.Select(x => new CartItemDto
            {
                ProductVariantId = x.Variant.Id,
                ProductId = x.Variant.Product.Id,
                ProductName = x.Variant.Product.ProductName,
                ImageUrl = x.Variant.Product.ProductImages.FirstOrDefault(x => x.IsPrimary)?.ImageUrl,
                Size = x.Variant.Size.ToString(),
                Sku = x.Variant.VariantSku,
                UnitPrice = x.Variant.UnitPrice,
                Quantity = x.Quantity,
                TotalPrice = x.Variant.UnitPrice * x.Quantity,
                AvailableStock = x.Variant.StockQuantity,
                IsAvailable = x.Variant.StockQuantity > 0,
                IsExceedStock = x.Quantity > x.Variant.StockQuantity
            }).ToList();
        }
    }

    public class GetProductsInCartEndPoint(AppDbContext db) : Endpoint<List<CartItemRequest>, List<CartItemDto>, GetProductInCartMapper> 
    {
        public override void Configure()
        {
            Post("");
            AllowAnonymous();
            Group<CartGroup>();
        }

        public override async Task HandleAsync(List<CartItemRequest> req, CancellationToken ct)
        {
            if(req.Count <= 0)
            {
                await Send.OkAsync([], ct);
                return;
            }

            var variantIds = req.Select(x => x.ProductVariantId).ToList();

            var variants = await db.ProductVariants
                .Where(v => variantIds.Contains(v.Id))
                .Include(v => v.Product)
                .ThenInclude(p => p.ProductImages)
                .AsNoTracking()
                .ToListAsync(ct);

            var combined = variants
                .Join(
                    req,                                            
                    v => v.Id,                                  
                    item => item.ProductVariantId,                  
                    (v, item) => (Variant: v, item.Quantity)        
                )
                .ToList();

            await Send.OkAsync(Map.FromEntity(combined), ct);
        }
    }
}
