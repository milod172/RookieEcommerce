using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.API.Features.ProductVariants
{
    public class GetVariantsQuery
    {
        [BindFrom("product_id")]
        public Guid ProductId { get; set; } = Guid.Empty;
    }

    public class GetProductVariantMapper : Mapper<GetVariantsQuery, List<ProductVariantDto>, List<ProductVariant>>
    {
        public ProductVariantDto MapToDto(ProductVariant e)
        {
            return new ProductVariantDto
            {
                Id = e.Id,
                ProductName = e.Product?.ProductName ?? string.Empty,
                ProductId = e.ProductId,
                Size = e.Size.ToString(),
                StockQuantity = e.StockQuantity,
                VariantSku = e.VariantSku,
                CreatedTime = e.CreatedTime,
                IsDeleted = e.IsDeleted,
                ModifiedTime = e.ModifiedTime,
                UnitPrice = e.UnitPrice,
            };
        }

        public override List<ProductVariantDto> FromEntity(List<ProductVariant> e)
        {
            return e.Select(MapToDto).ToList();
        }
    }

    public class GetProductVariants(AppDbContext db) : Endpoint<GetVariantsQuery, List<ProductVariantDto>, GetProductVariantMapper>
    {
        public override void Configure()
        {
            Get("products/{product_id}/variants");
            AllowAnonymous();
            Group<ProductVariantGroup>();
        }

        public override async Task HandleAsync(GetVariantsQuery req, CancellationToken ct)
        {
            var product = await db.Products.AnyAsync(p => p.Id == req.ProductId, ct);

            if (!product)
            {
                ThrowError("Không tìm thấy sản phẩm", statusCode: 404);
            }

            var variants = await db.ProductVariants
                .Include(v => v.Product)
                .AsNoTracking()
                .Where(v => v.ProductId == req.ProductId)
                .ToListAsync(ct);

            await Send.OkAsync(Map.FromEntity(variants), ct);
        }
    }
}

