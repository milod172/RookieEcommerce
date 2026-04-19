using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    
    public record UpdateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } = 0;
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
        public Guid? CategoryId { get; set; }
    }

    public class UpdateProductValidator : Validator<UpdateProductRequest>
    {
        public UpdateProductValidator()
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

    public class UpdateProductMapper : Mapper<UpdateProductRequest, ProductDto, Product>
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
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime,
            };
        }
    }

    public class UpdateProduct(IProductRepository productRepository) : Endpoint<UpdateProductRequest, ProductDto, UpdateProductMapper>
    { 
        public override void Configure()
        {
            Put("{id}");
            AllowAnonymous();
            //RequireAuthorization()
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
        {
            
            var product = await productRepository.FindAsync(Route<Guid>("id"), ct);
            if (product == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            Map.UpdateEntity(req, product);

           
            await productRepository.UpdateAsync(product, ct);
            var updatedProduct = Map.FromEntity(product);

            await Send.OkAsync(updatedProduct, ct);

        }
    }
        
}

