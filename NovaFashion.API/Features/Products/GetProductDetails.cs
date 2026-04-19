using FastEndpoints;
using NovaFashion.API.Entities;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{

    public class GetProductDetailsMapper : Mapper<EmptyRequest, ProductDto, Product>
    {
        public override ProductDto FromEntity(Product e) 
        {
            return new ProductDto {
                Id = e.Id,
                ProductName = e.ProductName,
                Description = e.Description,
                UnitPrice = e.UnitPrice,
                Details = e.Details,
                TotalQuantity = e.TotalQuantity,
                Sku = e.Sku,
                CategoryId = e.CategoryId != null ? e.CategoryId.Value : Guid.Empty,
                CategoryName = e.Category != null ? e.Category.CategoryName : string.Empty,
                CreatedTime = e.CreatedTime
            };
        }

    }
    public class GetProductDetails(IProductRepository productRepository) : EndpointWithoutRequest<ProductDto, GetProductDetailsMapper>
    {
        
        public override void Configure()
        {
            Get("{id}");
            AllowAnonymous();
            //RequireAuthorization()
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var product = await productRepository.FindAsync(Route<Guid>("id"), ct);

            if (product == null) { 
                await Send.NotFoundAsync(ct);
                return;
            }

            var productDto = Map.FromEntity(product); 

            await Send.OkAsync(productDto, ct);
        }
    }
}
