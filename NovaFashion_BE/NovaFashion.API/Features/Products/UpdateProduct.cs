using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public record UpdateProductRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } = 0;
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
        public Guid? CategoryId { get; set; }
    }

    public class UpdateProductValidator : Validator<UpdateProductRequest>
    {
        public const string ProductNameRequired = "Tên sản phẩm không được để trống";
        public const string DescriptionRequired = "Mô tả không được để trống";
        public const string DescriptionTooLong = "Mô tả không được vượt quá 500 ký tự";
        public const string DetailsTooLong = "Chi tiết sản phẩm không được vượt quá 1000 ký tự";
        public const string TotalQuantityInvalid = "Số lượng tổng phải lớn hơn hoặc bằng 0";
        public const string UnitPriceMustBeGreaterThanZero = "Giá phải lớn hơn 0";
        public const string UnitPriceTooLarge = "Giá quá lớn, vui lòng điều chỉnh lại";

        public UpdateProductValidator()
        {

            RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage(ProductNameRequired);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(DescriptionRequired)
                .MaximumLength(500)
                .WithMessage(DescriptionTooLong);

            RuleFor(x => x.Details)
                .MaximumLength(1000)
                .WithMessage(DetailsTooLong);

            RuleFor(x => x.TotalQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage(TotalQuantityInvalid);

            RuleFor(x => x.UnitPrice)
               .GreaterThan(0)
               .WithMessage(UnitPriceMustBeGreaterThanZero)
               .LessThanOrEqualTo(1_000_000_000)
               .WithMessage(UnitPriceTooLarge);
        }
    }

    public class UpdateProductMapper : Mapper<UpdateProductRequest, ProductDetailsDto, Product>
    {
        public override Product UpdateEntity(UpdateProductRequest r, Product e)
        {
            e.ProductName = r.ProductName;
            e.Description = r.Description;
            e.UnitPrice = r.UnitPrice;
            e.Details = r.Details;
            e.TotalQuantity = r.TotalQuantity;
            e.CategoryId = r.CategoryId;

            return e;
        }

        public override ProductDetailsDto FromEntity(Product e)
        {
            return new ProductDetailsDto
            {
                Id = e.Id,
                ProductName = e.ProductName,
                Description = e.Description,
                UnitPrice = e.UnitPrice,
                Details = e.Details,
                TotalQuantity = e.TotalQuantity,
                CategoryId = e.CategoryId != null ? e.CategoryId.Value : Guid.Empty,
                CategoryName = e.Category != null ? e.Category.CategoryName : string.Empty,
                Sku = e.Sku,
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime,
            };
        }
    }

    public class UpdateProduct(AppDbContext db) : Endpoint<UpdateProductRequest, ProductDetailsDto, UpdateProductMapper>
    {
        public override void Configure()
        {
            Put("{id}");
            AllowAnonymous();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
        {
            var product = await db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

            if (product == null)
            {
                ThrowError("Không tìm thấy sản phẩm", statusCode: 404);
            }

            Map.UpdateEntity(req, product);
            db.Products.Update(product);
            await db.SaveChangesAsync(ct);

            await Send.OkAsync(Map.FromEntity(product), ct);
        }
    }

}

