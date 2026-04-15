using FastEndpoints;
using NovaFashion.API.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public class DeleteProduct : EndpointWithoutRequest
    {
        private readonly AppDbContext _context;
        public DeleteProduct(AppDbContext context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Delete("/api/products/{id}");
            AllowAnonymous();
            //RequireAuthorization()
            Description(x => x
                .WithName("Delete Product")
                .Produces<ProductDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var product = _context.Products.Find(Route<Guid>("id"));
            if (product == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
            
            await Send.OkAsync(null, ct);
        }
    }
}
