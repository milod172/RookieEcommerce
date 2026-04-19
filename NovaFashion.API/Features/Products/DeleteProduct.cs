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
            //RequireAuthorization()
            Group<ProductGroup>();
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
