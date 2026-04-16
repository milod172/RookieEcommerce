using FastEndpoints;
using FluentValidation;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.API.Persistence;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Features.Products
{
    public record CreateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
    }

    public class CreateProductValidator : Validator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            RuleFor(x => x.Details)
                .MaximumLength(1000).WithMessage("Details cannot exceed 1000 characters.");
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(1_000_000_000).WithMessage("Price is too large");
        }
    }

    public class CreateProduct : Endpoint<CreateProductRequest>
    {
        private readonly AppDbContext _context;

        public CreateProduct(AppDbContext context)
        {
            _context = context;
        }
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
            var product = new Product
            {
                ProductName = req.ProductName,
                Description = req.Description,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(ct);
            

            var response = new ProductDto
            {
                Id = product.Id,                    
                ProductName = product.ProductName,
                Description = product.Description,
                CreatedTime = product.CreatedTime,
                CreatedBy = product.CreatedBy
            };

            await Send.CreatedAtAsync("GetProductDetails", product.Id, response,cancellation: ct);
        }
    }
}
