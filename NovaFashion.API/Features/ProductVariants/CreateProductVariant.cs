using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.API.Features.ProductVariants
{
    public record CreateProductVariantRequest
    {
        [BindFrom("product_id")]
        [JsonSchemaIgnore]
        public Guid ProductId { get; init; }
        public Size Size { get; init; }
        public int StockQuantity { get; init; }
        public decimal UnitPrice { get; init; }
    }

    public class CreateProductVariantValidator : Validator<CreateProductVariantRequest>
    {
        public const string SizeRequired = "Kích thước không được để trống";
        public const string StockQuantityInvalid = "Số lượng tồn kho phải lớn hơn hoặc bằng 0";
        public const string UnitPriceMustBeGreaterThanZero = "Giá phải lớn hơn 0";
        public const string UnitPriceTooLarge = "Giá quá lớn, vui lòng điều chỉnh lại";

        public CreateProductVariantValidator()
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

    public class ProductVariantMapper : Mapper<CreateProductVariantRequest, ProductVariantDto, ProductVariant>
    {
        public override ProductVariant ToEntity(CreateProductVariantRequest r)
        {
            return new ProductVariant
            {
                ProductId = r.ProductId,
                Size = r.Size,
                StockQuantity = r.StockQuantity,
                UnitPrice = r.UnitPrice
            };
        }

        public override ProductVariantDto FromEntity(ProductVariant e)
        {
            return new ProductVariantDto
            {
                Id = e.Id,
                ProductId = e.ProductId,
                ProductName = e.Product.ProductName,
                Size = e.Size.ToString(),
                VariantSku = e.VariantSku,
                StockQuantity = e.StockQuantity,
                UnitPrice = e.UnitPrice,
                CreatedTime = e.CreatedTime
            };
        }
    }

    public class CreateProductVariant(AppDbContext db) : Endpoint<CreateProductVariantRequest, ProductVariantDto, ProductVariantMapper>
    {   
        public override void Configure()
        {
            Post("products/{product_id}/variants");
            Group<ProductVariantGroup>();        
        }

        public override async Task HandleAsync(CreateProductVariantRequest req, CancellationToken ct)
        {
            var product = await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);

            if (product == null)
            {
                AddError("Không tìm thấy sản phẩm");
                await Send.NotFoundAsync(ct);
                return;
            }

            // 2. Sum all existing variants' StockQuantity for this product
            var existingTotalStock = await db.ProductVariants
                .Where(v => v.ProductId == req.ProductId)
                .SumAsync(v => (int?)v.StockQuantity, ct) ?? 0;

            // 3. Check if adding new variant would exceed TotalQuantity
            var projectedTotal = existingTotalStock + req.StockQuantity;

            if (projectedTotal > product.TotalQuantity)
            {
                AddError(
                    x => x.StockQuantity,
                    $"Tổng số lượng tồn kho của các biến thể ({projectedTotal}) " +
                    $"đang vượt quá tổng số lượng sản phẩm ({product.TotalQuantity})"
                );
                await Send.ErrorsAsync(400, ct);
                return;
            }

            var variant = Map.ToEntity(req);
            variant.VariantSku = variant.GenerateVariantSku(product.ProductName, product.Id);

            db.ProductVariants.Add(variant);
            await db.SaveChangesAsync(ct);
            var response = Map.FromEntity(variant);

            await Send.CreatedAsync(response, ct);
        }
    }
}
