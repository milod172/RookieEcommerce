using FastEndpoints;
using FluentValidation;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.API.Entities;
using NovaFashion.API.Persistence;

namespace NovaFashion.API.Features.Products
{
    public record CreateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateProductValidator : Validator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        }
    }

    public class CreateProduct : Endpoint<CreateProductRequest, ProductDto>
    {
        private readonly AppDbContext _context;

        public CreateProduct(AppDbContext context)
        {
            _context = context;
        }
        public override void Configure()
        {
            Post("/api/products");
            AllowAnonymous(); 
            //RequireAuthorization()
            Description(x => x
                .WithName("Create Product")
                .Produces<ProductDto>(StatusCodes.Status201Created)
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
            var result = await _context.SaveChangesAsync(ct);
            

            var response = new ProductDto
            {
                Id = product.Id,                    
                ProductName = product.ProductName,
                Description = product.Description,
                CreatedTime = product.CreatedTime,
                CreatedBy = product.CreatedBy
            };

            await Send.CreatedAtAsync("",
            null,
            response,
            cancellation: ct);
        }
    }
}
