using FastEndpoints;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public class DeleteProduct(IProductRepository productRepository) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Delete("{id}");
            AllowAnonymous();
            Group<ProductGroup>();
            //RequireAuthorization()
            Description(x => x
                .WithName("DeleteProduct")
                .Produces<ProductDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var product = await productRepository.FindAsync(Route<Guid>("id"), ct);
            if (product == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
            await productRepository.DeleteAsync(product, ct);
            
            
            await Send.OkAsync(null, ct);
        }
    }
}
