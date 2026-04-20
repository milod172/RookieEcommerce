using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public record CreateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } = 0;
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
        public Guid? CategoryId { get; set; }
    }

    public class CreateProductValidator : Validator<CreateProductRequest>
    {
        public const string ProductNameRequired = "Tên sản phẩm không được để trống";
        public const string DescriptionRequired = "Mô tả không được để trống";
        public const string DescriptionTooLong = "Mô tả không được vượt quá 500 ký tự";
        public const string DetailsTooLong = "Chi tiết sản phẩm không được vượt quá 1000 ký tự";
        public const string UnitPriceMustBeGreaterThanZero = "Giá phải lớn hơn 0";
        public const string UnitPriceTooLarge = "Giá quá lớn, vui lòng điều chỉnh lại";
        public CreateProductValidator()
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

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage(UnitPriceMustBeGreaterThanZero)
                .LessThanOrEqualTo(1_000_000_000)
                .WithMessage(UnitPriceTooLarge);
        }
    }

    public class CreateProductMapper : Mapper<CreateProductRequest, ProductDetailsDto, Product>
    {
        public override Product ToEntity(CreateProductRequest r)
        {
            return new Product
            {
                ProductName = r.ProductName,
                Description = r.Description,
                UnitPrice = r.UnitPrice,
                Details = r.Details,
                TotalQuantity = r.TotalQuantity,
                CategoryId = r.CategoryId
            };
        }

        public override ProductDetailsDto FromEntity(Product e)
        {
            return new ProductDetailsDto
            {
                Id = e.Id,
                ProductName = e.ProductName,
                UnitPrice = e.UnitPrice,
                Description = e.Description,
                Details = e.Details,
                TotalQuantity = e.TotalQuantity,
                CategoryId = e.CategoryId != null ? e.CategoryId.Value : Guid.Empty,
                CategoryName = e.Category != null ? e.Category.CategoryName : string.Empty,
                Sku = e.Sku,
                CreatedTime = e.CreatedTime
            };
        }
    }
    public class CreateProduct(AppDbContext db) : Endpoint<CreateProductRequest, ProductDetailsDto, CreateProductMapper>
    {
        public override void Configure()
        {
            Post("");
            AllowAnonymous();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
        {
            var product = Map.ToEntity(req);
            product.Sku = product.GenerateSku();

            db.Products.Add(product);
            await db.SaveChangesAsync(ct);

            var response = Map.FromEntity(product);
            await Send.CreatedAtAsync("GetProductDetails", new { id = product.Id }, response, cancellation: ct);
        }
    }
}
