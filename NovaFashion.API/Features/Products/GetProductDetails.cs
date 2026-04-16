using FastEndpoints;
using NovaFashion.API.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public class GetProductDetails : EndpointWithoutRequest<ProductDto>
    {
        private readonly AppDbContext _context;

        public GetProductDetails(AppDbContext context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Get("{id}");
            Group<ProductGroup>();
            AllowAnonymous();
            //RequireAuthorization()
            Description(x => x
                .WithName("GetProductDetails")
                .Produces<ProductDto>(StatusCodes.Status200OK));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var product = _context.Products.Find(Route<Guid>("id"));

            if (product == null) { 
                await Send.NotFoundAsync(ct);
                return;
            }

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
