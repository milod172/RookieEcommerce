using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.API.Features.ProductVariants
{
    public record UpdateProductVariantRequest
    {
        [BindFrom("product_id")]
        [JsonSchemaIgnore]
        public Guid ProductId { get; init; }
        [BindFrom("variant_id")]
        [JsonSchemaIgnore]
        public Guid VariantId { get; init; }
        public Size Size { get; init; }
        public int StockQuantity { get; init; }
        public decimal UnitPrice { get; init; }
    }

    public class UpdateProductVariantValidator : Validator<UpdateProductVariantRequest>
    {
        public const string SizeRequired = "Kích thước không được để trống";
        public const string StockQuantityInvalid = "Số lượng tồn kho phải lớn hơn hoặc bằng 0";
        public const string UnitPriceMustBeGreaterThanZero = "Giá phải lớn hơn 0";
        public const string UnitPriceTooLarge = "Giá quá lớn, vui lòng điều chỉnh lại";

        public UpdateProductVariantValidator()
        {
            RuleFor(x => x.Size)
                .NotEmpty()
                .WithMessage(SizeRequired);

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage(StockQuantityInvalid);

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage(UnitPriceMustBeGreaterThanZero)
                .LessThanOrEqualTo(1_000_000_000)
                .WithMessage(UnitPriceTooLarge);
        }
    }

    public class UpdateProductVariantMapper : Mapper<UpdateProductVariantRequest, ProductVariantDto, ProductVariant>
    {
        public override ProductVariant UpdateEntity(UpdateProductVariantRequest r, ProductVariant e)
        {
            e.Size = r.Size;
            e.StockQuantity = r.StockQuantity;
            e.UnitPrice = r.UnitPrice;
            return e;
        }
        public override ProductVariantDto FromEntity(ProductVariant e)
        {
            return new ProductVariantDto
            {
                Id = e.Id,
                ProductId = e.ProductId,
                Size = e.Size.ToString(),
                StockQuantity = e.StockQuantity,
                UnitPrice = e.UnitPrice,
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime
            };
        }

    }
    public class UpdateProductVariant(AppDbContext db) : Endpoint<UpdateProductVariantRequest, ProductVariantDto, UpdateProductVariantMapper>
    {
        public override void Configure()
        {
            Put("products/{product_id}/variants/{variant_id}");
            Group<ProductVariantGroup>();
        }

        public override async Task HandleAsync(UpdateProductVariantRequest req, CancellationToken ct)
        {
            var variant = await db.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == req.VariantId && v.ProductId == req.ProductId, ct);

            if (variant is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            var product = await db.Products
           .AsNoTracking()
           .FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);

            // Sum all OTHER variants (exclude the one being updated)
            var otherVariantsTotal = await db.ProductVariants
                .Where(v => v.ProductId == req.ProductId && v.Id != variant.Id)
                .SumAsync(v => (int?)v.StockQuantity, ct) ?? 0;

            // Check projected total — using new StockQuantity value
            var projectedTotal = otherVariantsTotal + req.StockQuantity;

            if (projectedTotal > product!.TotalQuantity)
            {
                AddError(
                    x => x.StockQuantity,
                    $"Tổng số lượng tồn kho của các biến thể ({projectedTotal}) " +
                    $"vượt quá số lượng sản phẩm ({product.TotalQuantity})"
                );
                await Send.ErrorsAsync(400, ct);
                return;
            }

            Map.UpdateEntity(req, variant);
            db.ProductVariants.Update(variant);
            await db.SaveChangesAsync(ct);
            var response = Map.FromEntity(variant);

            await Send.OkAsync(response, ct);
        }
    }
}
