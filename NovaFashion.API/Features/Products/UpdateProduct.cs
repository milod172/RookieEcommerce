using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    
    public record UpdateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateProductValidator : Validator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        }
    }

    public class UpdateProduct : Endpoint<UpdateProductRequest, ProductDto>
    {
        private readonly AppDbContext _context;
        public UpdateProduct(AppDbContext context)
        {
            _context = context;
        }
        public override void Configure()
        {
            Put("{id}");
            Group<ProductGroup>();
            AllowAnonymous();
            //RequireAuthorization()
            Description(x => x
                .WithName("UpdateProduct")
                .Produces<ProductDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest));
        }

        public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
        {
            
            var product = _context.Products.Find(Route<Guid>("id"));
            if (product == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            product.ProductName = req.ProductName;
            product.Description = req.Description;

            await _context.SaveChangesAsync(ct);
            
            var productDto = new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description
            };

            await Send.OkAsync(productDto, ct);

        }
    }
        
}

