using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.Products
{
    public class ProductGroup : Group
    {
        public ProductGroup()
        {
            Configure("products", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Product"));
            });
        }
    }
}
