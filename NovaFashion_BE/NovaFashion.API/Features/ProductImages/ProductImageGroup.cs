using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.ProductImages
{
    public class ProductImageGroup : Group
    {
        public ProductImageGroup()
        {
            Configure("", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Product Image")
                    .WithGroupName("Product Image"));
                ep.AllowAnonymous();
            });
        }
    }
}
