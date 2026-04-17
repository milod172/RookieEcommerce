using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
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
        public CreateProductValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được để trống")
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");
            RuleFor(x => x.Details)
                .MaximumLength(1000).WithMessage("Chi tiết sản phẩm không được vượt quá 1000 ký tự");
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Giá phải lớn hơn 0")
                .LessThanOrEqualTo(1000000000).WithMessage("Giá quá lớn, vui lòng điều chỉnh lại");
        }
    }

    public class CreateProductMapper : Mapper<CreateProductRequest, ProductDto, Product>
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

        public override ProductDto FromEntity(Product e)
        {
            return new ProductDto
            {
                Id = e.Id,
                ProductName = e.ProductName,
                UnitPrice = e.UnitPrice,
                Details = e.Details,
                TotalQuantity = e.TotalQuantity,
                CategoryId = e.CategoryId != null ? e.CategoryId.Value : Guid.Empty,
                CategoryName = e.Category != null ? e.Category.CategoryName : string.Empty,
                Sku = e.Sku,
                CreatedTime = e.CreatedTime
            };
        }
    }
    public class CreateProduct(IProductRepository productRepository) : Endpoint<CreateProductRequest, ProductDto, CreateProductMapper>
    {
        public override void Configure()
        {
            Post("");
            Group<ProductGroup>();
            AllowAnonymous();
            //RequireAuthorization()
            Description(x => x
                .WithName("CreateProduct")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest));
        }

        public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
        {

            var product = Map.ToEntity(req);
            product.Sku = product.GenerateSku();

            await productRepository.AddAsync(product, ct);

            var response = Map.FromEntity(product);
            await Send.CreatedAtAsync("GetProductDetails", new { id = product.Id }, response,cancellation: ct);
        }
    }
}
