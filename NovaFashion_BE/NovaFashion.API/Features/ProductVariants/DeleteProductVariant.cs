using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;

namespace NovaFashion.API.Features.ProductVariants
{
    public record DeleteProductVariantRequest
    {
        [BindFrom("product_id")]
        [JsonSchemaIgnore]
        public Guid ProductId { get; init; }

        [BindFrom("variant_id")]
        [JsonSchemaIgnore]
        public Guid VariantId { get; init; }
    }

    public class DeleteProductVariant(AppDbContext db) : Endpoint<DeleteProductVariantRequest>
    {
        public override void Configure()
        {
            Delete("products/{product_id}/variants/{variant_id}");
            Group<ProductVariantGroup>();
        }

        public override async Task HandleAsync(DeleteProductVariantRequest req, CancellationToken ct)
        {
            var productExists = await db.Products.AnyAsync(p => p.Id == req.ProductId, ct);
            if (!productExists) {
                ThrowError("Không tìm thấy sản phẩm", statusCode: 404);
            }

            var variant = await db.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == req.VariantId && v.ProductId == req.ProductId, ct);

            if (variant is null)
            {
                ThrowError("Không tìm thấy biến thể bên trong sản phẩm", statusCode: 404);
            }

            db.ProductVariants.Remove(variant);
            await db.SaveChangesAsync(ct);

            await Send.OkAsync("Xóa biến thể sản phẩm thành công", ct);
        }
    }
}
